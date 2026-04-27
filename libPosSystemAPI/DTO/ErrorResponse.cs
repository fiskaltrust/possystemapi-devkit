using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
namespace fiskaltrust.DevKit.POSSystemAPI.lib.DTO
{
    public class ErrorResponse
    {
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;
        [JsonPropertyName("status")]
        public int Status { get; set; } = 0;

        [JsonPropertyName("detail")]
        public string Detail { get; set; } = string.Empty;

        public override string ToString()
        {
            return $"ErrorResponse: Title='{Title}', Status='{Status}', Detail='{Detail}'";
        }
    }
}