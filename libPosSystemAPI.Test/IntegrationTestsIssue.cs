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
    /// System Tests for the ftPosAPI library in connection with a real POS System API endpoint and InStore App instance with configured DummyPaymentProvider
    /// </summary>
    public class IntegrationTestsIssue : IntegrationTestsBase
    {
        public IntegrationTestsIssue(ITestOutputHelper output) : base(output)
        {
        }

        [IntegrationFact]
        public async Task TestIssue()
        {
            Guid ftCashboxID = await InitFtPosSystemAPIClient();

            Guid operationID = Guid.NewGuid();
            string cbReceiptReference = "TestCbReceiptReference_" + Guid.NewGuid();
            ReceiptCaseBuilder rcBuilder = new ReceiptCaseBuilder();
            rcBuilder.SetCountry("AT");
            rcBuilder.SetReceiptCase(ReceiptCase.PointOfSaleReceipt0x0001);
            List<ChargeItem> chargeItems = new List<ChargeItem>();
            List<PayItem> payItems = new List<PayItem>();
            

            ExecutedResult<IssueResponse> payResult = await ftPosAPI.Issue.IssueAsync(
                new ReceiptRequest(cbReceiptReference, rcBuilder, chargeItems, payItems), new ReceiptResponse()                
                {
                    /**************
                    Attention - this is mostly dummy data for testing purposes only - not sure why it works at all to be honest
                    ***************/
                    ftCashBoxID = ftCashboxID,
                    ftQueueID = Guid.NewGuid(),
                    ftQueueItemID = Guid.NewGuid(),
                    ftQueueRow = 1,
                    ftReceiptMoment = DateTime.UtcNow,
                    cbReceiptReference = cbReceiptReference,
                    ftCashBoxIdentification = "TestCashBoxIdentification",
                    ftReceiptIdentification = "TestReceiptIdentification"
                }, 
                operationID
            );

            Assert.True(payResult.Executed, "IssueAsync - simple - failed: " + payResult.ErrorMessage);
            Assert.True(string.IsNullOrEmpty(payResult.ErrorMessage));
            Assert.NotNull(payResult.Operation);
            Assert.True(payResult.Operation.IsSuccess);
            Assert.Equal(operationID, payResult.Operation.OperationID);
            IssueResponse issueResponse = await payResult.Operation.GetResponseAsAsync();
            Assert.NotNull(issueResponse);
        }        
    }
}
