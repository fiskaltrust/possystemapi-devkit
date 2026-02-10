namespace fiskaltrust.DevKit.POSSystemAPI.lib.PosAPIClient
{
    public class ExecutedResult<T> where T : class
    {
        internal ExecutedResult(Operation<T> operation)
        {
            Operation = operation;
        }
        internal ExecutedResult(Operation<T> operation, string errorMessage)
        {
            Operation = operation;
            ErrorMessage = errorMessage;
        }

        public bool Executed => string.IsNullOrEmpty(ErrorMessage);
        public string ErrorMessage { get; private set; } = string.Empty;
        public Operation<T> Operation { get; private set; }
    }
}
