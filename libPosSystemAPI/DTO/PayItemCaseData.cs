using System.Collections.Generic;
using System.Text.Json;

namespace fiskaltrust.DevKit.POSSystemAPI.lib.DTO
{   public class PayItemCaseData
    {
        public Dictionary<string, JsonElement>? Provider { get; set; }
        public string[]? Receipt { get; set; }
    }
}