using System;
using System.Net;
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
            var result = await Utils.InitFtPosSystemAPIClient (httpTimeoutSeconds: 25);
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

                // IMPORTANT: We do provide the operationID here so we can query the status of this payment operation later
                //            via POST /PayResponse in case the initial /pay request fails due to communication issues.
                //
                //   Note: The backend identifies a payment operation by its operationID. POST /PayResponse with the same
                //         operationID returns the result of the original /pay operation - it does NOT trigger a new
                //         payment at the terminal. This is the recommended pattern to recover from communication
                //         failures.
                //         See https://docs.fiskaltrust.cloud/apis/pos-system-api for details.
                Guid operationId = Guid.NewGuid();
                var payItemRequest = new PayItemRequest
                {
                    Description = "Card",
                    Amount = amount,
                };

                // Initial /pay request:
                // - with the defined amount
                // - allow the target device to select the payment protocol (= use_auto; see payment config in target device / InStore App)
                // - the terminal ID is not defined here (null) so the request will be processed by all available terminals for the cashbox
                //     IMPORTANT: In a real setup you might want to define a specific terminal ID here to target a specific payment terminal device; especially when multiple payment terminals are registered for the same cashbox!
                // - we provide the operation ID so we can query the result later via /PayResponse
                Logger.LogInfo($"Sending payment request using operation ID: {operationId}");
                ExecutedResult<PayResponse> payResult = await ftPosAPI.Pay.PaymentAsync(payItemRequest, fiskaltrust.Payment.DTO.PaymentProtocol.use_auto, null, operationId);
                bool lastCallWasPayResponse = false;

                // In a real setup you might want to use a cancellation token or something similar to not endless retry.
                // We poll the backend until we either get a definitive response or the user aborts.
                while (true)
                {
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
                            // NO --> we received a response but it was a negative one.
                            //
                            // Special case: if the last call was /PayResponse and the backend returns 400 BadRequest,
                            // it means the operation ID is unknown to the backend. The most likely cause is that the
                            // original /pay request never reached the backend (so no payment was started, no terminal
                            // was contacted, no charge happened). We deliberately do NOT automatically resend /pay
                            // here - it is up to the POS integrator to decide how to handle this situation in their
                            // own POS (e.g. ask the cashier to confirm before retrying).
                            if (lastCallWasPayResponse && payResult.Operation.Response?.StatusCode == HttpStatusCode.BadRequest)
                            {
                                Logger.LogError("/PayResponse returned 400 BadRequest - operation ID is unknown to the backend.");
                                Logger.LogError("This most likely means the original /pay request never reached the backend, so no payment was started.");
                                Logger.LogError("It is up to the POS integration to decide whether to resend /pay (with the same or a new operation ID) or abort.");
                            }
                            else
                            {
                                Logger.LogInfo("Payment status request failed: " + payResult.ErrorMessage);
                            }
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
                        // NO --> communication issue with backend service. Lets query the result of the already started operation via POST /PayResponse using the same operation ID.
                        Logger.LogError($"Payment request failed: {payResult.ErrorMessage}");
                        Logger.LogInfo("Querying payment status via /PayResponse after 3s delay...");
                        await Task.Delay(3000);
                        payResult = await ftPosAPI.Pay.GetPayResponseAsync(operationId);
                        lastCallWasPayResponse = true;
                        continue;
                    }
                }
            }
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
