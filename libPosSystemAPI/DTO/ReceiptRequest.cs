using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using fiskaltrust.DevKit.POSSystemAPI.lib.PosAPIClient;

namespace fiskaltrust.DevKit.POSSystemAPI.lib.DTO
{
    /// <summary>
    /// The cash register transfers the data of an entire receipt request to the fiskaltrust.Middleware using the ReceiptRequest data structure.<br/>
    /// The field fiskaltrust receipt case (ftReceiptCase) is of the highest importance for the correct processing of the receipt.
    /// This field defines the receipt type, determines if the receipt has to be secured accordingly to the national law, and establishes the method to calculate the correct values for each national counter.
    /// </summary>
    public class ReceiptRequest : IJsonContentConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReceiptRequest"/> class.
        /// </summary>
        /// <param name="cbReceiptReference"><inheritdoc cref="cbReceiptReference" path="/summary"/></param>
        /// <param name="buildableReceiptCase">A configured <see cref="ReceiptCaseBuilder"/> that defines the receipt type and country.</param>
        /// <param name="cbChargeItems"><inheritdoc cref="cbChargeItems" path="/summary"/></param>
        /// <param name="cbPayItems"><inheritdoc cref="cbPayItems" path="/summary"/></param>
        public ReceiptRequest(string cbReceiptReference, ReceiptCaseBuilder buildableReceiptCase, IList<ChargeItem> cbChargeItems, IList<PayItem> cbPayItems)
        {
            this.cbReceiptReference = cbReceiptReference;
            this.cbChargeItems = cbChargeItems;
            // verify that empty ftPayItemIDs are null
            foreach (var payItem in cbPayItems)
            {
                if (payItem.ftPayItemId == "")
                {
                    payItem.ftPayItemId = null;
                }
            }
            this.cbPayItems = cbPayItems;
            this.ftReceiptCase = buildableReceiptCase.Build();
        }

        /// <summary>
        /// Reference number created by the caller.
        /// This value must be a unique string/receipt number related to the calling cash register.
        /// This string/receipt number is a unique primary key of the dataset of the cash register.
        /// </summary>
        [JsonPropertyName("cbReceiptReference")]
        public string cbReceiptReference { get; set; }

        /// <summary>
        /// Moment at which the receipt has been created by the cash register; must be provided in UTC.
        /// If not provided, the current UTC time at creation will be used.
        /// </summary>
        [JsonPropertyName("cbReceiptMoment")]
        public DateTime cbReceiptMoment { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// List of charge items related to services and products (e.g. sold products).
        /// </summary>
            [JsonPropertyName("cbChargeItems")]
        public IList<ChargeItem> cbChargeItems { get; set; }

        /// <summary>
        /// List of items related to payments.
        /// When using the /pay endpoint / <see cref="fiskaltrust.DevKit.POSSystemAPI.lib.ftPosAPI.Pay"/> for payment the ftPayItems from the payment response can be used without alteration.
        /// </summary>
        [JsonPropertyName("cbPayItems")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.Never)]
        public IList<PayItem> cbPayItems { get; set; }

        [JsonPropertyName("ftReceiptCase")]
        public ulong ftReceiptCase { get; set; }

        /// <summary>
        /// Reference cbReceiptReference of the previous receipt. Used to connect multiple requests for a single Business Case.
        /// </summary>
        [JsonPropertyName("cbPreviousReceiptReference")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? cbPreviousReceiptReference { get; set; }

        public JsonContent ToJsonContent()
        {
            return JsonContent.Create(this, options: JsonConfiguration.DefaultOptions);
        }

        /*
        [JsonProperty("cbTerminalID", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("cbTerminalID")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public string? cbTerminalID { get; set; }






        [JsonProperty("ftCashBoxID", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("ftCashBoxID")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Guid? ftCashBoxID { get; set; }

        [JsonProperty("ftPosSystemId", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("ftPosSystemId")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Guid? ftPosSystemId { get; set; }


        [JsonProperty("ftReceiptCaseData", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("ftReceiptCaseData")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public object? ftReceiptCaseData { get; set; }

        [JsonProperty("ftQueueID", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("ftQueueID")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Guid? ftQueueID { get; set; }


        [JsonProperty("cbReceiptAmount", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("cbReceiptAmount")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public decimal? cbReceiptAmount { get; set; }

        [JsonProperty("cbUser", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("cbUser")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public object? cbUser { get; set; }

        [JsonProperty("cbArea", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("cbArea")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public object? cbArea { get; set; }

        [JsonProperty("cbCustomer", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("cbCustomer")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public object? cbCustomer { get; set; }

        [JsonProperty("cbSettlement", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("cbSettlement")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public object? cbSettlement { get; set; }

        [Newtonsoft.Json.JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("Currency", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("Currency")]
        [System.Text.Json.Serialization.JsonConverter(typeof(JsonStringEnumConverter))]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public Currency Currency { get; set; }

        [JsonProperty("DecimalPrecisionMultiplier", DefaultValueHandling = DefaultValueHandling.Ignore)]
        [JsonPropertyName("DecimalPrecisionMultiplier")]
        [System.Text.Json.Serialization.JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
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