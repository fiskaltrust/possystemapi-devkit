using fiskaltrust.Payment.DTO;
using fiskaltrust.DevKit.POSSystemAPI.lib.DTO;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using fiskaltrust.DevKit.POSSystemAPI.lib.PosAPIClient;

namespace fiskaltrust.DevKit.POSSystemAPI.lib
{
    /// <summary>
    /// Provides payment operations for the POS System API.
    /// </summary>
    public class PosAPIPay
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cbPayItem"></param>
        /// <param name="terminalID">The terminal ID defines a specific target to send the request to.</param>
        /// <param name="protocol">The protocol (= target payment provider) to use.</param>
        /// <param name="operationId">
        /// If null (default) a new random operation ID will be generated.
        /// See https://docs.fiskaltrust.cloud/apis/pos-system-api about the meaning of the operation / operationID
        /// </param>
        /// <returns></returns>
        public async Task<ExecutedResult<PayResponse>> PaymentAsync(PayItemRequest cbPayItem, PaymentProtocol protocol = PaymentProtocol.use_auto, string? terminalID = null, Guid? operationId = null)
        {
            // https://docs.fiskaltrust.cloud/apis/pos-system-api#tag/SynchronAPI/paths/~1pay/post

            PaymentRequest payRequest = new PaymentRequest
            {
                Action = PayAction.payment,
                Protocol = protocol,
                cbPayItem = cbPayItem,
                cbTerminalId = terminalID,
            };            
            var rBuilder = new APIRequestBuilder<PaymentRequest, PayResponse>()
                .SetMethod(HttpMethod.Post)
                .SetPath("/pay")
                .SetContent(payRequest);
            if (operationId.HasValue)
            {
                rBuilder.SetOperationID(operationId.Value);
            }
            return await OperationExecutorImpl<PaymentRequest, PayResponse>.Instance.ExecuteOperationAsync(rBuilder);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cbPayItem">cbPayItem.Amount must be a negative value <0</param>
        /// <param name="protocol"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">If cbPayItem.Amount >= 0</exception>
        public async Task<ExecutedResult<PayResponse>> RefundAsync(PayItem cbPayItem, PaymentProtocol protocol = PaymentProtocol.use_auto)
        {
            // https://docs.fiskaltrust.cloud/apis/pos-system-api#tag/SynchronAPI/paths/~1pay/post
            if (cbPayItem.Amount >= 0)
            {
                throw new ArgumentException("Amount must be negative for refund operations", nameof(cbPayItem));
            }

            RefundCancelRequest payRequest = new RefundCancelRequest
            {
                Action = PayAction.refund,
                Protocol = protocol,
                cbPayItem = cbPayItem,
            };
            var rBuilder = new APIRequestBuilder<RefundCancelRequest, PayResponse>()
                .SetMethod(HttpMethod.Post)
                .SetPath("/pay")
                .SetContent(payRequest);
            return await OperationExecutorImpl<RefundCancelRequest, PayResponse>.Instance.ExecuteOperationAsync(rBuilder);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cbPayItem">cbPayItem.Amount must be a negative value <0</param>
        /// <param name="protocol"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">If cbPayItem.Amount >= 0</exception>
        public async Task<ExecutedResult<PayResponse>> CancelAsync(PayItem cbPayItem, PaymentProtocol protocol = PaymentProtocol.use_auto)
        {
            // https://docs.fiskaltrust.cloud/apis/pos-system-api#tag/SynchronAPI/paths/~1pay/post
            if (cbPayItem.Amount >= 0)
            {
                throw new ArgumentException("Amount must be negative for cancel operations", nameof(cbPayItem));
            }

            RefundCancelRequest refundCancelRequest = new RefundCancelRequest
            {
                Action = PayAction.cancel,
                Protocol = protocol,
                cbPayItem = cbPayItem,
            };
            var rBuilder = new APIRequestBuilder<RefundCancelRequest, PayResponse>()
                .SetMethod(HttpMethod.Post)
                .SetPath("/pay")
                .SetContent(refundCancelRequest);
            return await OperationExecutorImpl<RefundCancelRequest, PayResponse>.Instance.ExecuteOperationAsync(rBuilder);
        }
    }
}