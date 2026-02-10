using System;

namespace fiskaltrust.DevKit.POSSystemAPI.lib.DTO
{
    public class PayItemRequest
    {
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
}