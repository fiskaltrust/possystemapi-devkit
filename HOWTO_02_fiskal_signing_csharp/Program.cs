using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using fiskaltrust.DevKit.POSSystemAPI.lib;
using fiskaltrust.DevKit.POSSystemAPI.lib.DTO;
using fiskaltrust.DevKit.POSSystemAPI.lib.PosAPIClient;
using fiskaltrust.DevKit.POSSystemAPI.lib.PosAPIUtils;

namespace fiskaltrust.DevKit.POSSystemAPI.Howto.signing.GettingStarted
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var result = await Utils.InitFtPosSystemAPIClient ();
            if (!result.success)
            {
                Console.WriteLine(result.errorMsg);
            }
            else
            {
                Logger.LogInfo("fiskaltrust POS System API initialized successfully.");

                /////////////////////////////////////////////////////////////////////////////////////////////////
                // execute a simple payment
                Logger.LogInfo($"Executing a simple signing request...");

                // IMPORTANT: We do provide the operationID here to be able to retry the operation in case of failure
                // 
                //   Note: The backend will check the operationID and the payload of the request.
                //         And if it is identical to a previous request it will return the result of that previous request instead of executing a new payment request.
                Guid operationID = Guid.NewGuid();

                // In a real setup you might want to use a cancellation token or something similar to not endless retry.
                // We send the request until it either succeeds or we get a negative response from the backend. (= we retry if we do not get a response for whatever reason)
                //
                // IMPORTANT: To simplify the retry logic please check out HOWTO_08_pay_sign_issue and the ftPosAPIOperationRunner class there.
                while (true)
                {
                    Logger.LogInfo($"Sending sign request using operation ID: {operationID}");
                    string receiptRef = "TestReceipt_" + Guid.NewGuid();
                    ReceiptCaseBuilder receiptCaseBuilder = new ReceiptCaseBuilder()
                        .SetCountry("AT")
                        .SetReceiptCase(ReceiptCase.PointOfSaleReceipt0x0001);
                    List<ChargeItem> chargeItems = new List<ChargeItem>()
                    {
                        new ChargeItem()
                        {
                            Description = "Test Item 1",
                            Amount = 20.0m,
                            Quantity = 1,
                            VATRate = 20.0m,
                        },
                        new ChargeItem()
                        {
                            Description = "Test Item 2",
                            Amount = 10.0m,
                            Quantity = 2,
                            VATRate = 10.0m,
                        }
                    };
                    List<PayItem> payItems = new List<PayItem>()
                    {
                        new PayItem()
                        {
                            ftPayItemId = Guid.NewGuid().ToString(),
                            Amount = 40.0m,
                            Description = "Cash Payment",
                        }
                    };

                    ReceiptRequest receiptRequest = new ReceiptRequest(receiptRef, receiptCaseBuilder, chargeItems, payItems);
                    ExecutedResult<ReceiptResponse> signResult = await ftPosAPI.Sign.SignAsync(receiptRequest, operationID);
                    if (signResult.Executed)
                    {
                        if (signResult.Operation.IsSuccess)
                        {
                            ReceiptResponse receiptResponse = await signResult.Operation.GetResponseAsAsync();
                            Logger.LogInfo("Sign request successful:");
                            Utils.DumpToLogger(receiptResponse);
                            break;
                        }
                        else
                        {
                            Logger.LogInfo("Sign request failed: " + signResult.ErrorMessage);
                            ErrorResponse? errResp = await signResult.Operation.GetResponseErrorAsync();
                            if (errResp != null)
                            {
                                Logger.LogError(errResp.ToString());
                            }
                            else
                            {
                                Logger.LogError("No further error information received from backend.");
                            }
                            break;
                        }
                    }
                    else
                    {
                        Logger.LogError($"Sign request failed: {signResult.ErrorMessage}");
                        Logger.LogInfo("Retrying sign request after 3s delay...");
                        await Task.Delay(3000);
                        continue;
                    }
                }
            }
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
