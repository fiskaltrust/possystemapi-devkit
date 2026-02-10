using fiskaltrust.Payment.DTO;
using System;
using System.Text.Json.Serialization;

namespace fiskaltrust.DevKit.POSSystemAPI.lib.DTO
{
    public class PayResponse
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PaymentProtocol Protocol { get; set; }
        public string ftQueueID { get; set; } = string.Empty;
        public PayItem[] ftPayItems { get; set; } = Array.Empty<PayItem>(); // should always be set but if not included in response we would have null with potential NPE -> initialize with empty array
    }
}