using System;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace fiskaltrust.DevKit.POSSystemAPI.lib.DTO
{
    public class ChargeItem
    {
        [JsonPropertyName("Quantity")]
        public decimal Quantity { get; set; } = 1m;

        [JsonPropertyName("Description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Defines the (total) amount of the charge item. to get itemprice the amount need to be devided by quantity.
        /// </summary>
        [JsonPropertyName("Amount")]
        public decimal Amount { get; set; }

        [JsonPropertyName("VATRate")]
        public decimal VATRate { get; set; }

        /*
        [JsonProperty("ftChargeItemId", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("ftChargeItemId")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Guid? ftChargeItemId { get; set; }


        [JsonProperty("ftChargeItemCase", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("ftChargeItemCase")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public long? ftChargeItemCase { get; set; }

        [JsonProperty("ftChargeItemCaseData", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("ftChargeItemCaseData")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public object? ftChargeItemCaseData { get; set; }

        [JsonProperty("VATAmount", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("VATAmount")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public decimal? VATAmount { get; set; }

        [JsonProperty("Moment", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("Moment")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public DateTime? Moment { get; set; }

        [JsonProperty("Position", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("Position")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public decimal Position { get; set; }

        [JsonProperty("AccountNumber", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("AccountNumber")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public string? AccountNumber { get; set; }

        [JsonProperty("CostCenter", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("CostCenter")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public string? CostCenter { get; set; }

        [JsonProperty("ProductGroup", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("ProductGroup")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public string? ProductGroup { get; set; }

        [JsonProperty("ProductNumber", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("ProductNumber")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public string? ProductNumber { get; set; }

        [JsonProperty("ProductBarcode", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("ProductBarcode")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public string? ProductBarcode { get; set; }

        [JsonProperty("Unit", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("Unit")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public string? Unit { get; set; }

        [JsonProperty("UnitQuantity", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("UnitQuantity")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public decimal? UnitQuantity { get; set; }

        [JsonProperty("UnitPrice", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("UnitPrice")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [DataMember(EmitDefaultValue = false, IsRequired = false)]
        public decimal? UnitPrice { get; set; }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("Currency", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("Currency")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [DataMember(Order = 170, EmitDefaultValue = false, IsRequired = false)]
        public Currency Currency { get; set; }

        [JsonProperty("DecimalPrecisionMultiplier", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("DecimalPrecisionMultiplier")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [DataMember(Order = 180, EmitDefaultValue = false, IsRequired = false)]
        public int DecimalPrecisionMultiplierSerialization
        {
            get
            {
                if (DecimalPrecisionMultiplier != 1)
                {
                    return DecimalPrecisionMultiplier;
                }

                return 0;
            }
            set
            {
                DecimalPrecisionMultiplier = ((value == 0) ? 1 : value);
            }
        }

        [Newtonsoft.Json.JsonIgnore]
        [System.Text.Json.Serialization.JsonIgnore]
        public int DecimalPrecisionMultiplier { get; set; } = 1;
        */
    }
}