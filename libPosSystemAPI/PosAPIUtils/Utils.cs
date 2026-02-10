using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using fiskaltrust.DevKit.POSSystemAPI.lib.DTO;

namespace fiskaltrust.DevKit.POSSystemAPI.lib.PosAPIUtils
{
    public static class Utils
    {
        /// <summary>
        /// Tests if we are able to reach the POS System API.
        /// 1) check if we are able to resolve the domain
        /// 2) check if we are able to connect to the POS System API endpoint
        /// 3) check if we are able to execute a POST request to echo expecting to receive an UNAUTHORIZED response (since we do not provide any credentials here)
        /// </summary>
        /// <returns>true if successful --> you are set to use the API</returns>
        public static async Task<bool> PingFtPosApiAvailable(string posSystemAPIUrl = "https://possystem-api-sandbox.fiskaltrust.eu/v2")
        {
            try
            {
                Uri uri = new Uri(posSystemAPIUrl);

                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // Step 1: Check if we can resolve the domain
                Logger.LogDebug($"Step 1: Resolving domain {uri.Host}...");
                var hostEntry = await System.Net.Dns.GetHostEntryAsync(uri.Host);
                if (hostEntry.AddressList.Length == 0)
                {
                    Logger.LogError($"Failed to resolve domain: {uri.Host}");
                    return false;
                }
                var ipAddress = hostEntry.AddressList[0];
                Logger.LogDebug($"Domain resolved to {ipAddress} ({hostEntry.HostName})");

                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // Step 2: TCP connect to verify network connectivity
                // uri.Port returns the explicit port if specified, or the default port for the scheme (443 for https, 80 for http)
                int port = uri.Port;
                Logger.LogDebug($"Step 2: TCP connect to {ipAddress}:{port}...");
                using var tcpClient = new System.Net.Sockets.TcpClient();
                await tcpClient.ConnectAsync(ipAddress, port);
                if (!tcpClient.Connected)
                {
                    Logger.LogError($"Failed to establish TCP connection to {ipAddress}:{port}");
                    return false;
                }
                Logger.LogDebug($"TCP connection established to {ipAddress}:{port}");
                tcpClient.Close();

                ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                // Step 3: HTTP POST request to echo - expecting 400 BAD REQUEST because the authorization headers are not provided
                // Ensure URL ends with / for proper relative path resolution
                string baseUrl = posSystemAPIUrl.EndsWith("/") ? posSystemAPIUrl : posSystemAPIUrl + "/";
                Logger.LogDebug($"Step 3: Sending POST request to {baseUrl}echo...");
                using var httpClient = new System.Net.Http.HttpClient
                {
                    BaseAddress = new Uri(baseUrl),
                    Timeout = TimeSpan.FromSeconds(10)
                };

                // Send POST request without credentials - expecting 400 BadRequest
                using (var request = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Post, "echo"))
                {
                    var response = await httpClient.SendAsync(request);

                    // We expect 400 BadRequest since we're not providing credentials + detail message contains the "x-cashbox-id" info
                    if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        // check body
                        string responseBody = await response.Content.ReadAsStringAsync();
                        if (!responseBody.Contains("x-cashbox-id"))
                        {
                            Logger.LogError("Received 400 BadRequest but response body does not contain expected 'x-cashbox-id' info.");
                            return false;
                        }

                        Logger.LogDebug("Received expected 400 BadRequest response - API is reachable!");
                        return true;
                    }

                    // Any successful response (2xx) also means the API is reachable
                    if (response.IsSuccessStatusCode)
                    {
                        Logger.LogDebug($"Received successful response ({response.StatusCode}) - API is reachable!");
                        return true;
                    }

                    // Other responses might indicate issues but at least we connected
                    Logger.LogWarning($"Received unexpected status code: {response.StatusCode}");
                }
                return false;
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                Logger.LogError($"Network error: Unable to connect to the API endpoint. {ex.Message}");
                return false;
            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                Logger.LogError($"HTTP error: {ex.Message}");
                return false;
            }
            catch (TaskCanceledException)
            {
                Logger.LogError("Connection timed out while trying to reach the POS System API.");
                return false;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Unexpected error while pinging POS System API: {ex.Message}");
                return false;
            }
        }

        public static async Task<(bool success, string errorMsg)> InitFtPosSystemAPIClient ()
        {
            (Guid ftCashboxID, string ftCashboxAccessToken)? credentials = GetCashboxCredentialsFromEnvironment();
            if (credentials == null)
            {
                Logger.LogError("Failed to get cashbox credentials from environment variables.");
                return (false, "Please set the environment variables FISKALTRUST_CASHBOX_ACCESS_TOKEN and FISKALTRUST_CASHBOX_ID to valid values.");
            }

            string? posSystemAPIUrl = Environment.GetEnvironmentVariable("FISKALTRUST_POS_SYSTEM_API_URL");
            if (posSystemAPIUrl == null)
            {
                Logger.LogWarning("FISKALTRUST_POS_SYSTEM_API_URL not set, using default URL https://possystem-api-sandbox.fiskaltrust.eu/v2");
                posSystemAPIUrl = "https://possystem-api-sandbox.fiskaltrust.eu/v2";
            }
            else
            {
                Logger.LogInfo("Using POS System API URL from environment FISKALTRUST_POS_SYSTEM_API_URL: " + posSystemAPIUrl);
            }

            Logger.LogInfo("Initializing ftPosAPI for cashbox ID: " + credentials.Value.ftCashboxID);
            ftPosAPI.Init(credentials.Value.ftCashboxID, credentials.Value.ftCashboxAccessToken, posSystemAPIUrl, 75);

            (bool success, _) = await ftPosAPI.EchoAsync();
            if (!success)
            {
                return (false, "Failed to connect to " + ftPosAPI.POSSystemAPIUrl + " with the provided credentials. Please check the environment variables and your network connection.");
            }

            return (true, string.Empty);
        }

        public static (Guid ftCashboxID, string ftCashboxAccessToken)? GetCashboxCredentialsFromEnvironment()
        {
            string? ftCashboxAccessToken = Environment.GetEnvironmentVariable("FISKALTRUST_CASHBOX_ACCESS_TOKEN");
            string? ftCashboxIDStr = Environment.GetEnvironmentVariable("FISKALTRUST_CASHBOX_ID");

            if (string.IsNullOrEmpty(ftCashboxAccessToken) || string.IsNullOrEmpty(ftCashboxIDStr) || !Guid.TryParse(ftCashboxIDStr, out Guid ftCashboxID))
            {
                return null;
            }

            return (ftCashboxID, ftCashboxAccessToken);
        }

        public static void DumpToLogger(PayResponse payResp)
        {
            Logger.LogInfo("Payment successful! Queue ID: " + payResp.ftQueueID);        
            if (payResp.ftPayItems == null || payResp.ftPayItems.Length == 0)
            {
                Logger.LogWarning("WARNING: No pay items received in response!");
                return;
            }
            
            // pretty log the response (JSON)
            Logger.LogDebug("PayResponse: " + JsonSerializer.Serialize(payResp, new JsonSerializerOptions { WriteIndented = true }));

            foreach (PayItem ftPayItem in payResp.ftPayItems)
            {
                var payItemCaseData = ftPayItem.GetPayItemCaseData();

                Dictionary<string, JsonElement>? providerInfo = payItemCaseData?.Provider;
                string protocol = "unknown";
                if (providerInfo != null &&providerInfo.TryGetValue("Protocol", out JsonElement protocolValue))
                {
                    protocol = protocolValue.GetString() ?? "ERROR: invalid type";
                }
                Logger.LogInfo($"- {protocol}:");
                if (payItemCaseData?.Receipt == null)
                {
                    Logger.LogInfo("\t WARNING: No receipt info received!");
                }
                else
                {
                    string[]? payReceipt = ftPayItem.GetPayItemCaseData()?.Receipt;
                    if (payReceipt != null)
                    {
                        foreach (string line in payReceipt)
                        {
                            Logger.LogInfo($"\t{line}");
                        }
                    }
                }
            }
        }

        public static void DumpToLogger(ReceiptResponse receiptResponse)
        {
            Logger.LogInfo($"Receipt signed successfully! {receiptResponse.ftQueueID}/{receiptResponse.ftQueueItemID}/{receiptResponse.ftQueueRow}");            
            Logger.LogInfo("  ftState: " + receiptResponse.ftState);
            Logger.LogInfo("  ftReceiptIdentification: " + receiptResponse.ftReceiptIdentification);
            Logger.LogDebug("  ReceiptResponse: " + JsonSerializer.Serialize(receiptResponse, new JsonSerializerOptions { WriteIndented = true }));            
        }

        public static void DumpToLogger(IssueResponse issueResponse)
        {
            Logger.LogInfo($"Receipt issued successfully! {issueResponse.ftQueueID}/{issueResponse.ftQueueItemID}");            
            Logger.LogInfo("  DocumentURL: " + issueResponse.DocumentURL);
        }
    }
}