using System;
using System.Net.Http;
using System.Threading.Tasks;
using fiskaltrust.DevKit.POSSystemAPI.lib.DTO;
using fiskaltrust.DevKit.POSSystemAPI.lib.PosAPIClient;
using fiskaltrust.DevKit.POSSystemAPI.lib.PosAPIUtils;
using fiskaltrust.Payment.DTO;

namespace fiskaltrust.DevKit.POSSystemAPI.lib
{
    public abstract class RequestBuilderBase<TReq, TResp> where TResp : class where TReq : IJsonContentConverter
    {
        internal abstract APIRequestBuilder<TReq, TResp> Build();
    }

    public class PaymentRequestBuilder : RequestBuilderBase<PaymentRequest, PayResponse>
    {
        private readonly PayItemRequest _payItem;
        
        public PaymentRequestBuilder(PayItemRequest cbPayItem)
        {
            this._payItem = cbPayItem;
        }

        PaymentProtocol _protocol = PaymentProtocol.use_auto;
        public PaymentRequestBuilder SetProtocol(PaymentProtocol protocol)
        {
            this._protocol = protocol;
            return this;
        }

        string? _terminalID = null;
        public PaymentRequestBuilder SetTerminalID(string terminalID)
        {
            this._terminalID = terminalID;
            return this;
        }

        Guid? _operationID = null;
        public PaymentRequestBuilder SetOperationID(Guid operationID)
        {
            this._operationID = operationID;
            return this;
        }

        internal override APIRequestBuilder<PaymentRequest, PayResponse> Build()
        {
            PaymentRequest payRequest = new PaymentRequest
            {
                Action = PayAction.payment,
                Protocol = this._protocol,
                cbPayItem = this._payItem,
                cbTerminalId = this._terminalID,
            };
            var rBuilder = new APIRequestBuilder<PaymentRequest, PayResponse>()
                .SetMethod(HttpMethod.Post)
                .SetPath("/pay")
                .SetContent(payRequest);
            if (this._operationID.HasValue)
            {
                rBuilder.SetOperationID(this._operationID.Value);
            }
            return rBuilder;
        }
    }



    /// <summary>
    /// Executes an ftPosAPI operation with retry logic
    /// 1) Create a unique operation ID for the operation
    /// 2) Execute the operation via the ftPosAPI client library
    /// 3) If the operation fails due to communication issues retry the operation with the same operation ID
    /// 4) If the operation succeeds return the result
    /// </summary>
    public class ftPosAPIOperationRunner
    {
        public Guid OperationID { get; private set; } = Guid.NewGuid();

        /// <summary>
        /// Executes the provided function with retry logic to handle communication failures with the POS System API
        /// </summary>
        /// <typeparam name="TResp"></typeparam>
        /// <param name="executeSingleReq">
        /// A function that executes a single request and returns an ExecutedResult of TResp
        /// If it returns null the retry loop will exit and null will be returned.
        /// </param>
        /// <returns>Tuple
        /// resp = Received response of the operation or null if execution was cancelled (= exedcSingleReq(..) returns null)
        /// errorMsg = Error message if operation failed, empty string if operation succeeded or execution was cancelled
        /// </returns>
        public async Task<(TResp? resp, string errorMsg)> Execute<TResp>(Func<Task<ExecutedResult<TResp>?>> executeSingleReq) where TResp : class
        {
            while(true)
            {
                var result = await executeSingleReq();
                if (result == null)
                {
                    Logger.LogDebug("Received null result from executeSingleReq --> exiting retry loop");
                    return (null, "");
                }

                if (result.Executed)
                {
                    if (result.Operation.IsSuccess)
                    {
                        return (await result.Operation.GetResponseAsAsync(), "");
                    }
                    else
                    {
                        Logger.LogInfo("Operation failed: " + result.ErrorMessage);
                        ErrorResponse? errResp = await result.Operation.GetResponseErrorAsync();
                        if (errResp != null)
                        {
                            Logger.LogError(errResp.ToString());
                        }
                        return (null, result.ErrorMessage);
                    }
                }
                else
                {
                    Logger.LogInfo("Operation could not be executed: " + result.ErrorMessage);
                    Logger.LogInfo("Retrying request after 3 seconds...");
                    await Task.Delay(3000);
                }
            }
        }
    }
}