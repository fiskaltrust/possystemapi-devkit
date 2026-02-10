using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace fiskaltrust.DevKit.POSSystemAPI.lib.DTO
{
    /// <summary>
    /// fiskaltrust.Middleware sends back the processed data to the cash register through the receipt response.
    /// The data included in the request, such as header, service, pay items, and footer, will not be sent back.
    /// The returned data is added to the receipt as supplement to the data of the receipt request.
    /// </summary>
    public class ReceiptResponse : QueueItemBase
    {
        /// <summary>
        /// Human readable identification or serial number of the cash register related to the national regulation/law.
        /// This need to be printed on the receipt to identify the cash register within a merchant, and is unique over a single merchant.
        /// Do not mix up ftCashBoxId with ftCashBoxIdentification!
        /// </summary>
        [JsonPropertyName("ftCashBoxIdentification")]
        public string ftCashBoxIdentification { get; set; } = string.Empty;

        /// <summary>
        /// Human readable identification of the receipt related to the national regulation/law and the ftCashBoxIdentification/ Queue.
        /// This need to be printed on the receipt to identify it within a merchant and cash register.
        /// This always starts with 'ft', followed by the row number of the queue in hexadecimal, followed by '#', followed by national required or defined receipt numbering.
        /// </summary>
        [JsonPropertyName("ftReceiptIdentification")]
        public string ftReceiptIdentification { get; set; } = string.Empty;

        /// <summary>
        /// UTC Moment at which the receipt was processed by fiskaltrust.Middleware. Must printed on the receipt in local date/time.
        /// </summary>
        [JsonPropertyName("ftReceiptMoment")]
        public DateTime ftReceiptMoment { get; set; }
        
        /// <summary>
        /// The signature of the receipt must comply with the national law and is controlled via the ftReceiptCase provided in the <see cref="ReceiptRequest" />.
        /// The signature data returned in the response must be visualized on the receipt related to format instruction and to fiskaltrust reference.
        /// The signature entries can also be used to visualize hints and messages related to the fiskaltrust.SecurityMechanism.
        /// </summary>
        [JsonPropertyName("ftSignatures")]
        public List<SignatureItem> ftSignatures { get; set; } = new List<SignatureItem>();

        /// <summary>
        /// State of the fiskaltrust.Middleware. Contains encoded information about the country and state flags.
        /// Use <see cref="FtState"/> methods to decode the value.
        /// </summary>
        [JsonPropertyName("ftState")]
        public FtState ftState { get; set; } = new FtState(0);        

        /// <summary>
        /// Identification of the cash box / cash register.
        /// Required for a later /issue request.
        /// </summary>
        [JsonPropertyName("ftCashBoxID")]
        public Guid? ftCashBoxID { get; set; }
        
        /// <summary>
        /// Mirror from `ReceiptRequest` Reference number sent by the cash register.
        /// Required for a later /issue request.
        /// </summary>
        [JsonPropertyName("cbReceiptReference")]
        public string? cbReceiptReference { get; set; }
        /*

        [JsonProperty("ftQueueRow", DefaultValueHandling = DefaultValueHandling.Include)]
        [JsonPropertyName("ftQueueRow")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        [DataMember(EmitDefaultValue = true, IsRequired = true)]
        public long ftQueueRow { get; set; }




        [JsonProperty("cbTerminalID", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("cbTerminalID")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public string? cbTerminalID { get; set; }



        [JsonProperty("ftReceiptHeader", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("ftReceiptHeader")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public List<string>? ftReceiptHeader { get; set; }

        [JsonProperty("ftChargeItems", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("ftChargeItems")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public List<ChargeItem>? ftChargeItems { get; set; }

        [JsonProperty("ftChargeLines", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("ftChargeLines")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public List<string>? ftChargeLines { get; set; }

        [JsonProperty("ftPayItems", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("ftPayItems")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public List<PayItem>? ftPayItems { get; set; }

        [JsonProperty("ftPayLines", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("ftPayLines")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public List<string>? ftPayLines { get; set; }

        [JsonProperty("ftReceiptFooter", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("ftReceiptFooter")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public List<string>? ftReceiptFooter { get; set; }

        [JsonProperty("ftStateData", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("ftStateData")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public object? ftStateData { get; set; }
        */
    }

    public class SignatureItem
    {
        [JsonPropertyName("ftSignatureFormat")]
        [JsonConverter(typeof(SignatureFormatJsonConverter))]
        public SignatureFormat ftSignatureFormat { get; set; }

        [JsonPropertyName("ftSignatureType")]
        public ulong ftSignatureType { get; set; }

        [JsonPropertyName("Caption")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? Caption { get; set; }

        [JsonPropertyName("Data")]
        public string Data { get; set; }

        public override string ToString()
        {
            return $"SignatureItem: Format={ftSignatureFormat}, Type={ftSignatureType}, Caption={Caption}, DataLength={(Data != null ? Data.Length : 0)}";
        }
    }

    /// <summary>
    /// Format for displaying signature data according to fiskaltrust reference. Please refer to fiskaltrust documentation for details.
    /// The format contains two parts: FormatType (lower 16 bits) and Position (bits 16-31).
    /// </summary>
    public class SignatureFormat
    {
        private readonly ulong _value;

        public SignatureFormat(ulong value)
        {
            _value = value;
        }

        /// <summary>
        /// Format type enum for signature display formats.
        /// </summary>
        public enum SignatureFormatType : ulong
        {
            Unknown = 0,
            Text = 1,
            Link = 2,
            QRCode = 3,
            Code128 = 4,
            OcrA = 5,
            Pdf417 = 6,
            DataMatrix = 7,
            Aztec = 8,
            Ean8Barcode = 9,
            Ean13 = 10,
            UPCA = 11,
            Code39 = 12,
            Base64 = 13
        }

        /// <summary>
        /// Format type (lower 16 bits: 0x0000_0000_0000_FFFF)
        /// </summary>
        public SignatureFormatType FormatType => (SignatureFormatType)(_value & 0xFFFF);

        /// <summary>
        /// Position (bits 16-31: 0x0000_0000_FFFF_0000)
        /// </summary>
        public ulong Position => (_value >> 16) & 0xFFFF;

        public static implicit operator ulong(SignatureFormat format) => format._value;
        public static implicit operator SignatureFormat(ulong value) => new SignatureFormat(value);

        public override string ToString()
        {
            return $"FormatType={FormatType}, Position={Position}";
        }
    }

    /// <summary>
    /// JSON converter for SignatureFormat to serialize/deserialize as ulong.
    /// </summary>
    public class SignatureFormatJsonConverter : JsonConverter<SignatureFormat>
    {
        public override SignatureFormat Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return new SignatureFormat(reader.GetUInt64());
        }

        public override void Write(Utf8JsonWriter writer, SignatureFormat value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue((ulong)value);
        }
    }
}