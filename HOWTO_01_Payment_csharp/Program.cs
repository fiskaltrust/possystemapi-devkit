using System;
using System.Threading.Tasks;
using fiskaltrust.DevKit.POSSystemAPI.lib;
using fiskaltrust.DevKit.POSSystemAPI.lib.DTO;
using fiskaltrust.DevKit.POSSystemAPI.lib.PosAPIClient;
using fiskaltrust.DevKit.POSSystemAPI.lib.PosAPIUtils;

namespace fiskaltrust.DevKit.POSSystemAPI.Howto.Payment.GettingStarted
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
                Console.WriteLine("\n--- Simple Payment Example ---\n");
                // info about DummyPaymentProvider special amounts:
                Console.WriteLine("INFO: Using DummyPaymentProvider, the following special amounts can be used to simulate specific scenarios:");
                Console.WriteLine($"  - on payment request for {30000.10m:N2} --> DECLINED");
                Console.WriteLine($"  - on payment request for {30000.20m:N2} --> TIMEOUT");
                Console.WriteLine($"  - on payment request for {30000.40m:N2} --> CANCELLED BY USER");
                Console.WriteLine($"  - on payment request for {30000.50m:N2} --> success with added guest tip");
                Console.WriteLine($"  - on payment request for {30000.60m:N2} --> success after 1min delay");
                Console.WriteLine($"  - on payment request for {30000.70m:N2} --> success after 3min delay");
                Console.WriteLine($"  - on payment request for {30000.80m:N2} --> success after 6min delay");
                Console.WriteLine($"  - on payment request for {30000.90m:N2} --> success but the amount gets reduced to {15000.90m:N2} (simulating partial payment)");
                Console.WriteLine();

                Console.WriteLine("Please enter amount to charge:");
                if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
                {
                    Logger.LogInfo("Invalid / unparseable amount entered.");
                    return;
                }
                
                Logger.LogInfo($"Executing a simple payment of {amount} EUR via card...");

                // IMPORTANT: We do provide the operationID here to be able to retry the operation in case of failure
                // 
                //   Note: The backend will check the operationID and the payload of the request.
                //         And if it is identical to a previous request it will return the result of that previous request instead of executing a new payment request.
                Guid operationId = Guid.NewGuid();

                // In a real setup you might want to use a cancellation token or something similar to not endless retry.
                // We send the request until it either succeeds or we get a negative response from the backend. (= we retry if we do not get a response for whatever reason)
                //
                // IMPORTANT: To simplify the retry logic please check out HOWTO_08_pay_sign_issue and the ftPosAPIOperationRunner class there.
                while (true)
                {
                    Logger.LogInfo($"Sending payment request using operation ID: {operationId}");
                    // execute the payment
                    // - with with defined amount
                    // - allow the target device to select the payment protocol (= use_auto; see payment config in target device / InStore App)
                    // - the terminal ID is not defined here (null) so the request will be processed by all available terminals for the cashbox
                    //     IMPORTANT: In a real setup you might want to define a specific terminal ID here to target a specific payment terminal device; especially when multiple payment terminals are registered for the same cashbox!
                    // - we provide the operation ID to be able to retry in case of failure
                    var payItemRequest = new PayItemRequest
                    {
                        Description = "Card",
                        Amount = amount,
                    };
                    ExecutedResult<PayResponse> payResult = await ftPosAPI.Pay.PaymentAsync(payItemRequest, fiskaltrust.Payment.DTO.PaymentProtocol.use_auto, null, operationId);

                    /////////////////////////////////////////////////////////////////////////////////////////////////
                    // Check Result

                    // did the request execute correctly? (= did we receive a response)
                    if (payResult.Executed)
                    {
                        // YES --> was the payment successful?
                        if (payResult.Operation.IsSuccess)
                        {
                            // YES --> SUCCESS: Payment was successful
                            PayResponse payResp = await payResult.Operation.GetResponseAsAsync();
                            Utils.DumpToLogger(payResp, payItemRequest);
                            break;
                        }
                        else
                        {
                            // NO --> we received a response but it was a negative one --> do not retry, inform user (and the user might decide to retry manually)
                            Logger.LogInfo("Payment status request failed: " + payResult.ErrorMessage);
                            ErrorResponse? errResp = await payResult.Operation.GetResponseErrorAsync();
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
                        // NO --> communication issue with backend service --> retry
                        Logger.LogError($"Payment request failed: {payResult.ErrorMessage}");
                        Logger.LogInfo("Retrying payment request after 3s delay...");
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
