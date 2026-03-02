using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using fiskaltrust.DevKit.POSSystemAPI.lib;
using fiskaltrust.DevKit.POSSystemAPI.lib.DTO;
using fiskaltrust.DevKit.POSSystemAPI.lib.PosAPIClient;
using fiskaltrust.DevKit.POSSystemAPI.lib.PosAPIUtils;
using fiskaltrust.Payment.DTO;

namespace fiskaltrust.DevKit.POSSystemAPI.Howto.PaySignIssue
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

                ///////////////////////////////////////////////////////////////////////////////////////////////////////
                /// Create the "dummy" charge items                
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
                
                ///////////////////////////////////////////////////////////////////////////////////////////////////////
                /// 
                /// /pay - PAYMENT request
                /// 
                /// Pay the charge items
                /// 
                 
                // summarize the total amount to pay
                decimal totalAmount = chargeItems.Sum(ci => ci.Amount);
                Logger.LogInfo($"Total amount to pay: {totalAmount} EUR");

                PayItemRequest payRequest = new()
                {
                    Amount = totalAmount,
                    Description = "Card"
                };
                var payRunner = new ftPosAPIOperationRunner();
                (PayResponse? pResp, string errorMsg) = await payRunner.Execute<PayResponse>(async () =>
                {
 
                    return await ftPosAPI.Pay.PaymentAsync(payRequest, PaymentProtocol.use_auto, null, payRunner.OperationID);
                });
                
                if (pResp == null)
                {
                    Logger.LogError("Payment failed: " + errorMsg + ". Aborting further processing.");
                }
                else
                {
                    Logger.LogInfo("Payment succeeded.");
                    Utils.DumpToLogger(pResp, payRequest);

                    ///////////////////////////////////////////////////////////////////////////////////////////////////////
                    /// 
                    /// / /sign - SIGN receipt request
                    /// 
                    /// Create the receipt via /sign
                    /// 

                    var signRunner = new ftPosAPIOperationRunner();
                    string receiptReference = Guid.NewGuid().ToString();
                    Logger.LogInfo($"Creating receipt with reference: {receiptReference}");
                    ReceiptCaseBuilder receiptCaseBuilder = new ReceiptCaseBuilder()
                        .SetCountry("AT") // Austria
                        .SetReceiptCase(ReceiptCase.PointOfSaleReceipt0x0001);
                    ReceiptRequest receiptRequest = new ReceiptRequest(receiptReference, receiptCaseBuilder, chargeItems, pResp.ftPayItems);
                    (ReceiptResponse? rResp, errorMsg) = await signRunner.Execute<ReceiptResponse>(async () =>
                    {
                        return await ftPosAPI.Sign.SignAsync(receiptRequest, signRunner.OperationID);
                    });

                    if (rResp == null)
                    {
                        Logger.LogError("Signing receipt failed: " + errorMsg + ". Aborting further processing.");
                    }
                    else
                    {
                        Logger.LogInfo("Receipt signed successfully.");
                        Utils.DumpToLogger(rResp);

                        ///////////////////////////////////////////////////////////////////////////////////////////////////////
                        /// 
                        /// / /issue - ISSUE receipt request
                        /// 
                        /// Deliver the receipt via /issue
                        /// 
                        
                        var issueRunner = new ftPosAPIOperationRunner();
                        (IssueResponse? iResp, errorMsg) = await issueRunner.Execute<IssueResponse>(async () =>
                        {                            
                            return await ftPosAPI.Issue.IssueAsync(receiptRequest, rResp, issueRunner.OperationID);
                        });

                        if (iResp == null)
                        {
                            Logger.LogError("Issuing receipt failed: " + errorMsg + ". Aborting further processing.");
                        }
                        else
                        {
                            Logger.LogInfo("Receipt issued successfully.");
                            Utils.DumpToLogger(iResp);

                            ///////////////////////////////////////////////////////////////////////////////////////////////////////
                            ///
                            /// /check delivery status of issued receipt
                            ///
                            /// Check whether the issued receipt has been delivered (e.g. printed, emailed, etc.)
                            /// 

                            bool isDelivered = false;                                      
                            while (!isDelivered)
                            {
                                Logger.LogInfo("Checking delivery status of issued receipt...");
                                (isDelivered, IssueDeliveredResponse? deliveryDateils, errorMsg) = await ftPosAPI.Issue.IssueDeliveredAsync(iResp.ftQueueID, iResp.ftQueueItemID);
                                if (!string.IsNullOrEmpty(errorMsg))
                                {
                                    Logger.LogError("Checking delivery status failed: " + errorMsg + ". Aborting further processing.");
                                    break;
                                }
                                
                                if (isDelivered)
                                {
                                    Logger.LogInfo("Receipt has been delivered successfully.");
                                    Logger.LogDebug($"Delivery details - {deliveryDateils.state}: {deliveryDateils.message}");
                                }
                                else
                                {
                                    Logger.LogInfo("Receipt not yet delivered. Will check again in 3 seconds...");
                                    await Task.Delay(3000);
                                }
                            }                           
                        }
                    }
                }
            }
            
            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }
    }
}
