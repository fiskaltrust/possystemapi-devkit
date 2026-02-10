using Xunit.Abstractions;
using fiskaltrust.DevKit.POSSystemAPI.lib.PosAPIUtils;

namespace fiskaltrust.DevKit.POSSystemAPI.lib.Test
{
    /// <summary>
    /// Logger target that writes to xUnit's test output
    /// </summary>
    public class XUnitLoggerTarget : ILoggerTarget
    {
        private readonly ITestOutputHelper _output;

        public XUnitLoggerTarget(ITestOutputHelper output)
        {
            _output = output;
        }

        public void LogDebug(string message) => _output.WriteLine("[DEBUG] " + message);
        public void LogInfo(string message) => _output.WriteLine("[INFO] " + message);
        public void LogWarning(string message) => _output.WriteLine("[WARNING] " + message);
        public void LogError(string message) => _output.WriteLine("[ERROR] " + message);
    }
}