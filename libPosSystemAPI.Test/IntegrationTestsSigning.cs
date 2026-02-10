using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using fiskaltrust.DevKit.POSSystemAPI.lib.DTO;
using fiskaltrust.DevKit.POSSystemAPI.lib.PosAPIClient;
using Xunit;
using Xunit.Abstractions;

namespace fiskaltrust.DevKit.POSSystemAPI.lib.Test
{
    /// <summary>
    /// System Tests for the ftPosAPI library in connection with a real POS System API endpoint
    /// </summary>
    public class IntegrationTestsSigning : IntegrationTestsBase
    {
        public IntegrationTestsSigning(ITestOutputHelper output) : base(output)
        {
        }

        [IntegrationFact]
        public async Task TestSigningSimple()
        {
            await InitFtPosSystemAPIClient();

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
            Guid operationID = Guid.NewGuid();

            ExecutedResult<ReceiptResponse> signResult = await ftPosAPI.Sign.SignAsync(receiptRequest, operationID);
            Assert.NotNull(signResult);
            Assert.True(signResult.Executed, "SignAsync failed: " + signResult.ErrorMessage);
            Assert.True(string.IsNullOrEmpty(signResult.ErrorMessage));
            ReceiptResponse rresp = await signResult.Operation.GetResponseAsAsync();
            Assert.NotNull(rresp);
            Assert.NotEmpty(rresp.ftSignatures);
            Assert.NotEqual(FtStateStatus.Error, rresp.ftState.GetGlobalFlags());
            Assert.NotEqual(FtStateStatus.Fail, rresp.ftState.GetGlobalFlags());
        }        
    }
}
