using System;
using fiskaltrust.DevKit.POSSystemAPI.lib.DTO;
using fiskaltrust.DevKit.POSSystemAPI.lib.PosAPIUtils;

namespace fiskaltrust.DevKit.POSSystemAPI.lib.PosAPIClient
{
    public class ReceiptCaseBuilder
    {
        private ReceiptCase _receiptCase = 0;
        private string _countryCodeUpperCased = "";
        private const ulong CountryCodeMask = 0x00000000000000FF;

        /// <summary>
        /// Sets the receipt case for the builder.
        /// </summary>
        public ReceiptCaseBuilder SetReceiptCase(ReceiptCase receiptCase)
        {
            _receiptCase = receiptCase;
            return this;
        }

        /// <summary>
        /// Sets the country code for the receipt case.
        /// This will be encoded in the upper 16 bits of the receipt case ulong value and defines the country and therefore regulations context for the receipt.
        /// </summary>
        public ReceiptCaseBuilder SetCountry(string countryCode)
        {
            if (countryCode.Length != 2)
            {
                throw new ArgumentException("Country code must be a 2-letter ISO 3166-1 alpha-2 country code.");
            }
            countryCode = countryCode.ToUpper();
            switch (countryCode)
            {
                case "AT": // Austria
                case "BE": // Belgium
                case "DE": // Germany
                case "ES": // Spain
                case "FR": // France
                case "GR": // Greece
                case "IT": // Italy
                case "PT": // Portugal
                    break;
                default:
                    Logger.LogWarning($"Country code '{countryCode}' is not explicitly supported. Proceeding anyway.");
                    break;
            }

            _countryCodeUpperCased = countryCode;
            return this;
        }

        /// <summary>
        /// Builds the receipt case value as ulong.
        /// </summary>
        /// <returns>Receipt case value as ulong with country code set</returns>
        public ulong Build()
        {
            if (string.IsNullOrWhiteSpace(_countryCodeUpperCased))
            {
                throw new InvalidOperationException("Country code is not set or invalid. Please set a valid 2-letter country code using SetCountry().");
            }
            if (_receiptCase == ReceiptCase.UnknownReceipt0x0000)
            {
                Logger.LogWarning("Receipt case is set to UnknownReceipt0x0000. This may not be valid for actual receipt operations.");
            }

            ulong receiptCaseValue = ((ulong)(_countryCodeUpperCased[0] << 8) + (ulong)_countryCodeUpperCased[1]) << 48;
            receiptCaseValue += (ulong)_receiptCase;

            return receiptCaseValue;
        }
    }

    
}