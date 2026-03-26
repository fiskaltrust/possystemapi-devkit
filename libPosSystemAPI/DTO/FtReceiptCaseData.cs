using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace fiskaltrust.DevKit.POSSystemAPI.lib.DTO
{
    /// <summary>
    /// Optional additional data for the receipt case, passed as <c>ftReceiptCaseData</c> in a <see cref="ReceiptRequest"/>.
    /// Known properties are defined as typed fields. Any additional properties can be placed in
    /// <see cref="AdditionalProperties"/> and will be serialized as top-level JSON properties.
    /// </summary>
    public class FtReceiptCaseData
    {
        /// <summary>
        /// Lines of text to be printed on the receipt additionally to the standard receipt content.
        /// </summary>
        [JsonPropertyName("cbReceiptLines")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string[]? cbReceiptLines { get; set; }

        /// <summary>
        /// Additional arbitrary properties that will be serialized as top-level JSON fields
        /// alongside the known properties.
        /// </summary>
        [JsonExtensionData]
        public Dictionary<string, object?>? AdditionalProperties { get; set; }
    }
}
