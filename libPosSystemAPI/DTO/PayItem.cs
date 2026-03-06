using System;
using System.Text.Json.Serialization;

namespace fiskaltrust.DevKit.POSSystemAPI.lib.DTO
{
    public class PayItem
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? ftPayItemId { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }

        public ulong? ftPayItemCase { get; set; }

        public object? ftPayItemCaseData { get; set; }

        public DateTime Moment { get; set; }
    }
}