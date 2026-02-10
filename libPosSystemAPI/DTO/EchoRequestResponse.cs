using System.Net.Http.Json;
using System.Text.Json.Serialization;
using fiskaltrust.DevKit.POSSystemAPI.lib.PosAPIClient;

namespace fiskaltrust.DevKit.POSSystemAPI.lib.DTO
{
    public class EchoRequestResponse : IJsonContentConverter
    {
        [JsonPropertyName("Message")]
        public string Message { get; set; } = string.Empty;

        public JsonContent ToJsonContent()
        {
            return JsonContent.Create(this, options: JsonConfiguration.DefaultOptions);
        }
    }
}