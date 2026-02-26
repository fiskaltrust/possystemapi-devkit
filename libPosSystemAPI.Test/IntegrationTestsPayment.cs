using System;
using System.Threading.Tasks;
using fiskaltrust.DevKit.POSSystemAPI.lib.DTO;
using fiskaltrust.DevKit.POSSystemAPI.lib.PosAPIClient;
using fiskaltrust.DevKit.POSSystemAPI.lib.PosAPIUtils;
using Xunit;
using Xunit.Abstractions;

namespace fiskaltrust.DevKit.POSSystemAPI.lib.Test
{
    /// <summary>
    /// System Tests for the ftPosAPI library in connection with a real POS System API endpoint and InStore App instance with configured DummyPaymentProvider
    /// </summary>
    public class IntegrationTestsPayment : IntegrationTestsBase
    {
        public IntegrationTestsPayment(ITestOutputHelper output) : base(output)
        {
        }

        [IntegrationFact]
        public async Task TestPayment()
        {
            await InitFtPosSystemAPIClient();

            Guid operationID = Guid.NewGuid();

            ExecutedResult<PayResponse> payResult = await ftPosAPI.Pay.PaymentAsync(new DTO.PayItemRequest()
            {
                Description = "Test Item",
                Amount = 10.0m,
            }, Payment.DTO.PaymentProtocol.use_auto, null, operationID);

            Assert.True(payResult.Executed, "PaymentAsync failed: " + payResult.ErrorMessage);
            Assert.True(string.IsNullOrEmpty(payResult.ErrorMessage));
            Assert.NotNull(payResult.Operation);
            Assert.True(payResult.Operation.IsSuccess);
            Assert.Equal(operationID, payResult.Operation.OperationID);
            PayResponse payResponse = await payResult.Operation.GetResponseAsAsync();
            Assert.NotNull(payResponse);
            Assert.True(payResponse.Protocol == Payment.DTO.PaymentProtocol.use_auto);
            Assert.NotNull(payResponse.ftPayItems);
            // dummy payment provider should return exactly one pay item
            Assert.Single(payResponse.ftPayItems);

            PayItem pi = payResponse!.ftPayItems![0];
            Assert.Equal(10.0m, pi.Amount);
            Assert.NotEmpty(pi.ftPayItemId);
            Assert.True(pi.ftPayItemCase > 0);
            Assert.NotNull(pi.ftPayItemCaseData);
            PayItemCaseData? piCaseData = pi.GetPayItemCaseData();
            Assert.NotNull(piCaseData);

            Assert.True(piCaseData.Provider["Protocol"].GetString() == Payment.DTO.PaymentProtocol.test.ToString(), "Either someting is not working or the InStore App does not have DummyPaymentProvider configured.");
            Assert.True(piCaseData.Provider["Action"].GetString() == Payment.DTO.PayAction.payment.ToString());
        }

        
    }
}
