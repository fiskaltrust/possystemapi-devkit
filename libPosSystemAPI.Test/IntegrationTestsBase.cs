using System;
using System.Threading.Tasks;
using fiskaltrust.DevKit.POSSystemAPI.lib.DTO;
using fiskaltrust.DevKit.POSSystemAPI.lib.PosAPIClient;
using fiskaltrust.DevKit.POSSystemAPI.lib.PosAPIUtils;
using Microsoft.VisualBasic;
using Xunit;
using Xunit.Abstractions;

namespace fiskaltrust.DevKit.POSSystemAPI.lib.Test
{
    /// <summary>
    /// Custom attribute for integration tests that require real environment setup.
    /// These tests are skipped by default. Set environment variable RUN_INTEGRATION_TESTS=true to enable.
    /// </summary>
    public class IntegrationFactAttribute : FactAttribute
    {
        public IntegrationFactAttribute()
        {
            if (Environment.GetEnvironmentVariable("RUN_INTEGRATION_TESTS") != "true")
            {
                Skip = "Integration test - Set RUN_INTEGRATION_TESTS=true and configure credentials to run; REQUIRES special setup to be able to pass (InStore App with DummyPaymentProvider, POS System API endpoint, paired cashbox)";
            }
        }
    }

    /// <summary>
    /// System Tests for the ftPosAPI library in connection with a real POS System API endpoint and InStore App instance with configured DummyPaymentProvider
    /// </summary>
    public abstract class IntegrationTestsBase
    {
        public IntegrationTestsBase(ITestOutputHelper output)
        {
            Logger.SetLoggerTarget(new XUnitLoggerTarget(output));

            if (Environment.GetEnvironmentVariable("FISKALTRUST_POS_SYSTEM_API_URL") == null)
            {
                Environment.SetEnvironmentVariable("FISKALTRUST_POS_SYSTEM_API_URL", "https://possystem-api-sandbox.fiskaltrust.eu/v2");
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Basic connectivity tests to know if we can even reach the POS System API endpoints

        [IntegrationFact]
        public async Task TestPingFtPosApiAvailable()
        {
            var url = "https://possystem-api-sandbox.fiskaltrust.eu/v2";
            //var url = "http://localhost:8080/v2";
            bool isAvailable = await Utils.PingFtPosApiAvailable(url);
            Assert.True(isAvailable, "POS System API is not reachable. Check your network connection.");
        }

        [IntegrationFact]
        public async Task TestInit()
        {
            (bool success, string errorMessage) = await Utils.InitFtPosSystemAPIClient ();
            Assert.True(success, "Failed to initialize ftPosAPI: " + errorMessage);
            Assert.Empty(errorMessage);
        }
        
        [IntegrationFact]
        public async Task TestEcho()
        {
            await InitFtPosSystemAPIClient();

            string testMessage = "Hello from SystemTests at " + DateTime.UtcNow.ToString("o");
            (bool success, DTO.EchoRequestResponse? response) = await ftPosAPI.EchoAsync(testMessage);
            Assert.True(success, "EchoAsync failed.");
            Assert.NotNull(response);
            Assert.Equal(testMessage, response!.Message);
        }
        
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Lib functions usable in other tests

        /// <summary>
        /// Initialize the ftPosAPI client from environment variables
        /// </summary>
        /// <returns>ftCashboxID used</returns>
        protected async Task<Guid> InitFtPosSystemAPIClient()
        {
            (Guid ftCashboxID, string ftCashboxAccessToken)? credentials = Utils.GetCashboxCredentialsFromEnvironment();
            string? posSystemAPIUrl = Environment.GetEnvironmentVariable("FISKALTRUST_POS_SYSTEM_API_URL");
            ftPosAPI.Init(credentials!.Value.ftCashboxID, credentials.Value.ftCashboxAccessToken, Guid.Parse("00000000-0000-0000-0000-000000000000"), posSystemAPIUrl!, 75);
            return credentials.Value.ftCashboxID;
        }
    }
}
