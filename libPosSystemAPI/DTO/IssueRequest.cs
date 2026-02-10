
using System;
using System.Net.Http.Json;
using fiskaltrust.DevKit.POSSystemAPI.lib.PosAPIClient;

namespace fiskaltrust.DevKit.POSSystemAPI.lib.DTO
{
    public class IssueRequest : IJsonContentConverter
    {
        public IssueRequest(ReceiptRequest receiptRequest, ReceiptResponse receiptResponse)
        {
            ReceiptRequest = receiptRequest;
            ReceiptResponse = receiptResponse;
        }

        public ReceiptRequest ReceiptRequest { get; private set; }
        public ReceiptResponse ReceiptResponse { get; private set; }

        public JsonContent ToJsonContent()
        {
            return JsonContent.Create(this, options: JsonConfiguration.DefaultOptions);
        }
    }

}