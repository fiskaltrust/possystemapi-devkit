using System;
using System.Threading.Tasks;

namespace fiskaltrust.DevKit.POSSystemAPI.lib.PosAPIClient
{
    /// <summary>
    /// Executes operations against the POS API.
    /// Instance can be retrieved via <see cref="OperationExecutorImpl.Instance"/>.
    /// For test purpose a mock implementation can be set.
    /// </summary>
    public interface IOperationExecutor<TReq, TResp> where TResp : class where TReq : IJsonContentConverter
    {
        Task<ExecutedResult<TResp>> ExecuteOperationAsync(APIRequestBuilder<TReq, TResp> rBuilder);
    }

    internal class OperationExecutorImpl<TReq, TResp> : IOperationExecutor<TReq, TResp> where TResp : class where TReq : IJsonContentConverter
    {
        public static IOperationExecutor<TReq, TResp> Instance { get; set; } = new OperationExecutorImpl<TReq, TResp>();

        public async Task<ExecutedResult<TResp>> ExecuteOperationAsync(APIRequestBuilder<TReq, TResp> rBuilder)
        {
            Operation<TResp> op = rBuilder.Build();

            try
            {
                await op.Execute();
                return new ExecutedResult<TResp>(op);
            }
            catch (Exception ex)
            {           
                return new ExecutedResult<TResp>(op, ex.Message);
            } 
        }
    }
}