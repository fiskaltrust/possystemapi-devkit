using System;

namespace fiskaltrust.DevKit.POSSystemAPI.lib.PosAPIClient
{
    public class CashboxCredentials
    {
        public Guid CashboxID { get; set; }
        public string AccessToken { get; set; } = string.Empty;
    }
}

