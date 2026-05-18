using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace fiskaltrust.DevKit.POSSystemAPI.lib.DTO
{
    /// <summary>
    /// Provides some details about how the delivery was executed for debug purpose.
    /// </summary>
    public class IssueDeliveredResponse
    {
        /// <summary>
        /// A message describing how the delivery was executed.
        /// </summary>
        /// <example>""Receipt was printed at 01/30/2026 00:03:57. DeliveryMethod: customeraccepted"</example>
        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Additional arbitrary properties that will be serialized as top-level JSON fields
        /// alongside the known properties.
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object?>? AdditionalProperties { get; set; }    }
}
