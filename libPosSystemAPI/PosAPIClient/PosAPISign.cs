using fiskaltrust.Payment.DTO;
using fiskaltrust.DevKit.POSSystemAPI.lib.DTO;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using fiskaltrust.DevKit.POSSystemAPI.lib.PosAPIClient;

namespace fiskaltrust.DevKit.POSSystemAPI.lib.PosAPIClient
{

    public class PosAPISign
    {
        /// <summary>
        /// This method can be used to sign different types of receipts according to local fiscalization regulations.
        /// After signing the receipt according to the fiscal law, this method synchronously returns the data that need to be printed onto the receipt.
        /// The format of the receipt request is documented in the Middleware API docs, and the exact behavior of the method is determined by the cases
        /// sent within the properties (see ftReceiptCase, ftChargeItemCase and ftPayItemCase).
        /// </summary>
        /// <param name="receiptRequest">The receipt to sign.</param>
        /// <param name="operationId"><inheritdoc cref="PosAPIPay.PaymentAsync" path="/param[@name='operationId']"/></param>
        /// <returns></returns>
        public async Task<ExecutedResult<ReceiptResponse>> SignAsync(ReceiptRequest receiptRequest, Guid? operationId = null)
        {
            var builder = new APIRequestBuilder<ReceiptRequest, ReceiptResponse>()
                .SetMethod(HttpMethod.Post)
                .SetPath("/sign")
                .SetContent(receiptRequest);

            if (operationId.HasValue)
            {
                builder.SetOperationID(operationId.Value);
            }

            return await OperationExecutorImpl<ReceiptRequest, ReceiptResponse>.Instance.ExecuteOperationAsync(builder);
        }
    }
}