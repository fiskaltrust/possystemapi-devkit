using System;
using System.Text.Json;
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

        [IntegrationFact]
        public async Task TestPayResponseByOperationId()
        {
            await InitFtPosSystemAPIClient();

            Guid operationID = Guid.NewGuid();

            ExecutedResult<PayResponse> paymentResult = await ftPosAPI.Pay.PaymentAsync(new DTO.PayItemRequest()
            {
                Description = "Test Item PayResponse",
                Amount = 11.0m,
            }, Payment.DTO.PaymentProtocol.use_auto, null, operationID);

            Assert.True(paymentResult.Executed, "PaymentAsync failed: " + paymentResult.ErrorMessage);
            Assert.True(paymentResult.Operation.IsSuccess, "PaymentAsync operation was not successful");

            PayResponse paymentResponse = await paymentResult.Operation.GetResponseAsAsync();
            Assert.NotNull(paymentResponse);

            ExecutedResult<PayResponse> payResponseResult = await ftPosAPI.Pay.GetPayResponseAsync(operationID);

            Assert.True(payResponseResult.Executed, "PayResponseAsync failed: " + payResponseResult.ErrorMessage);
            Assert.True(string.IsNullOrEmpty(payResponseResult.ErrorMessage));
            Assert.NotNull(payResponseResult.Operation);
            Assert.True(payResponseResult.Operation.IsSuccess);
            Assert.Equal(operationID, payResponseResult.Operation.OperationID);

            PayResponse payResponse = await payResponseResult.Operation.GetResponseAsAsync();
            Assert.NotNull(payResponse);
            Assert.NotNull(payResponse.ftPayItems);
            Assert.NotEmpty(payResponse.ftPayItems);
            Assert.Equal(Payment.DTO.PaymentProtocol.use_auto, payResponse.Protocol);

            Assert.Equal(paymentResponse.Protocol, payResponse.Protocol);
            Assert.Equal(paymentResponse.ftQueueID, payResponse.ftQueueID);
            Assert.Equal(paymentResponse.ftPayItems.Length, payResponse.ftPayItems.Length);

            for (int i = 0; i < paymentResponse.ftPayItems.Length; i++)
            {
                PayItem expected = paymentResponse.ftPayItems[i];
                PayItem actual = payResponse.ftPayItems[i];

                Assert.Equal(expected.ftPayItemId, actual.ftPayItemId);
                Assert.Equal(expected.Description, actual.Description);
                Assert.Equal(expected.Amount, actual.Amount);
                Assert.Equal(expected.ftPayItemCase, actual.ftPayItemCase);
                Assert.Equal(expected.Moment, actual.Moment);

                string expectedCaseData = JsonSerializer.Serialize(expected.ftPayItemCaseData);
                string actualCaseData = JsonSerializer.Serialize(actual.ftPayItemCaseData);
                Assert.Equal(expectedCaseData, actualCaseData);
            }
        }

        [IntegrationFact]
        public async Task TestPayResponseByUnknownOperationId()
        {
            await InitFtPosSystemAPIClient();

            Guid unknownOperationID = Guid.NewGuid();

            ExecutedResult<PayResponse> payResponseResult = await ftPosAPI.Pay.GetPayResponseAsync(unknownOperationID);

            Assert.True(payResponseResult.Executed, "GetPayResponseAsync failed to execute: " + payResponseResult.ErrorMessage);
            Assert.NotNull(payResponseResult.Operation);
            Assert.Equal(unknownOperationID, payResponseResult.Operation.OperationID);
            Assert.False(payResponseResult.Operation.IsSuccess, "Unknown operation ID should not return success.");

            ErrorResponse? errorResponse = await payResponseResult.Operation.GetResponseErrorAsync();
            Assert.NotNull(errorResponse);

            Assert.Equal(404, errorResponse.Status);
            Assert.NotNull(errorResponse.Title);
            Assert.NotNull(errorResponse.Detail);
        }

        
    }
}
