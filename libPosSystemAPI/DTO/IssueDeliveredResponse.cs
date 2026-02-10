namespace fiskaltrust.DevKit.POSSystemAPI.lib.DTO
{
    /// <summary>
    /// Provides some details about how the delivery was executed for debug purpose.
    /// </summary>
    public class IssueDeliveredResponse
    {
        public string state { get; set; } = string.Empty;
        public string message { get; set; } = string.Empty;
    }
}
