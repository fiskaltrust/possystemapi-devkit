using fiskaltrust.DevKit.POSSystemAPI.lib.PosAPIClient;
using fiskaltrust.Payment.DTO;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace fiskaltrust.DevKit.POSSystemAPI.lib.DTO
{
    public class PaymentRequest : IJsonContentConverter
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)] // required so even if it has default value it must be included
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PayAction Action { get; set; }
        
        [JsonIgnore(Condition = JsonIgnoreCondition.Never)] // required so even if it has default value it must be included
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PaymentProtocol Protocol { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.Never)] // required so even if it has default value it must be included
        public required PayItemRequest cbPayItem { get; set; }

        /// <summary>
        /// Allows to identify a specific target device/terminal or group that should process the request.
        /// </summary>
        public string? cbTerminalID { get; set; }

        public JsonContent ToJsonContent()
        {
            if (string.IsNullOrEmpty(cbTerminalID)) cbTerminalID = null;
            return JsonContent.Create(this, options: JsonConfiguration.DefaultOptions);
        }
    }
}