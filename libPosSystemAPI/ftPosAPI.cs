using fiskaltrust.Payment.DTO;
using fiskaltrust.DevKit.POSSystemAPI.lib.DTO;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using fiskaltrust.DevKit.POSSystemAPI.lib.PosAPIClient;
using fiskaltrust.DevKit.POSSystemAPI.lib.PosAPIUtils;

namespace fiskaltrust.DevKit.POSSystemAPI.lib
{
    // TODO: We can nicely execute requests BUT in case of a failure how can we redo a previous operation using the same operation ID?
    //          (for example if we crash during processing while waiting for a result or there is a power outage)

    // TODO: How can a customer use /sign but then print the receipt manually by themselves (e.g. on a real printer or with additional information)?
    //          (if they do not want to use QR code, send by email or sms or get escpos receipt)     

    /// <summary>
    /// ftPosAPI main class
    /// </summary>
    public static class ftPosAPI
    {
        internal static string POSSystemAPIUrl {get; private set; } = string.Empty;
        internal static HttpClient? Client { get; private set; }
        internal static string PathPrefix { get; private set; } = string.Empty;
        internal static Guid CashBoxId { get; private set; }

        /// <summary>
        /// Initializes the ftPosAPI client with the given parameters.
        /// </summary>
        /// <param name="cashboxID"></param>
        /// <param name="cashboxAccessToken"></param>
        /// <param name="posSystemAPIUrl"></param>
        /// <param name="httpTimeoutSeconds">Timeout for HTTP requests in seconds.</param>
        public static void Init(Guid cashboxID, string cashboxAccessToken, string posSystemAPIUrl = "https://possystem-api-sandbox.fiskaltrust.eu/v2", int httpTimeoutSeconds = 60)
        {
            POSSystemAPIUrl = posSystemAPIUrl;
            CashBoxId = cashboxID;
            Uri uri = new Uri(posSystemAPIUrl);
            PathPrefix = uri.AbsolutePath;

            Client = new HttpClient
            {
                BaseAddress = uri,
                Timeout = TimeSpan.FromSeconds(httpTimeoutSeconds)
            };
            Client.DefaultRequestHeaders.Add("x-cashbox-id", CashBoxId.ToString());
            Client.DefaultRequestHeaders.Add("x-cashbox-accesstoken", cashboxAccessToken);
        }

        public static async Task<(bool success, EchoRequestResponse? responseMessage)> EchoAsync(string message = "Hello fiskaltrust POS System API!")
        {
            // https://docs.fiskaltrust.cloud/apis/pos-system-api#tag/SynchronAPI/paths/~1echo/post

            Operation<EchoRequestResponse> op = new APIRequestBuilder<EchoRequestResponse, EchoRequestResponse>()
                .SetMethod(HttpMethod.Post)
                .SetPath("/echo")
                .SetContent(new EchoRequestResponse { Message = message })
                .Build();
            try
            {
                await op.Execute();
                if (!op.IsSuccess)
                {
                    Logger.LogError($"Request to {op.Request.RequestUri} failed: { op.Response?.StatusCode }, { await op.GetResponseContentAsync()}");
                }
                return (op.IsSuccess, await op.GetResponseAsAsync());
            }
            catch (Exception ex)
            {
                Logger.LogError($"Request to {op.Request.RequestUri} failed: {ex.Message}");
                return (false, null);
            }
        }

        public static PosAPIPay Pay { get; } = new PosAPIPay();

        public static PosAPISign Sign { get; } = new PosAPISign();

        public static PosAPIIssue Issue { get; } = new PosAPIIssue();
    }

}
