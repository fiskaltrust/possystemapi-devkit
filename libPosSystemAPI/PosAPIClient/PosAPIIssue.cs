using fiskaltrust.Payment.DTO;
using fiskaltrust.DevKit.POSSystemAPI.lib.DTO;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using fiskaltrust.DevKit.POSSystemAPI.lib.PosAPIClient;
using System.Net;

namespace fiskaltrust.DevKit.POSSystemAPI.lib.PosAPIClient
{

    public class PosAPIIssue
    {
        /// <summary>
        /// Issue request based on receipt request and receipt response from /sign request
        /// </summary>
        /// <param name="receiptRequest"></param>
        /// <param name="receiptResponse"></param>
        /// <param name="operationId"></param>
        /// <returns></returns>
        public async Task<ExecutedResult<IssueResponse>> IssueAsync(ReceiptRequest receiptRequest, ReceiptResponse receiptResponse, Guid? operationId = null)
        {
            var builder = new APIRequestBuilder<IssueRequest, IssueResponse>()
                .SetMethod(HttpMethod.Post)
                .SetPath("/issue")
                .SetContent(new IssueRequest(receiptRequest, receiptResponse));

            if (operationId.HasValue)
            {
                builder.SetOperationID(operationId.Value);
            }

            return await OperationExecutorImpl<IssueRequest, IssueResponse>.Instance.ExecuteOperationAsync(builder);
        }


        /// <summary>
        /// Check whether the issued receipt has been delivered (e.g. printed, emailed, etc.)
        /// </summary>
        /// <param name="ftQueueID">The queue ID of the issued receipt</param>
        /// <param name="ftQueueItemID">The queue item ID of the issued receipt</param>
        /// <returns>
        /// <bool>isDelivered</bool>: true if the receipt has been delivered, false otherwise
        /// <IssueDeliveredResponse>deliveryDateils</IssueDeliveredResponse>: details about the delivery (null if not delivered yet)
        /// <string>errorMsg</string>: error message if the operation failed, empty string otherwise
        /// </returns>
        public async Task<(bool isDelivered, IssueDeliveredResponse? deliveryDateils, string errorMsg)> IssueDeliveredAsync(Guid ftQueueID, Guid ftQueueItemID)
        {
            using (var req = new HttpRequestMessage(HttpMethod.Get, ftPosAPI.Client.BaseAddress + $"/issue/{ftQueueID}/{ftQueueItemID}/delivered"))
            {
                HttpResponseMessage resp = await ftPosAPI.Client.SendAsync(req);
                if (resp.StatusCode == HttpStatusCode.OK)
                {
                    // delivered
                    var deliveryDetails = await resp.Content.ReadFromJsonAsync<IssueDeliveredResponse>();
                    return (true, deliveryDetails!, string.Empty);
                }
                else if (resp.StatusCode == HttpStatusCode.NoContent)
                {
                    // not yet delivered
                    return (false, null, string.Empty);
                }
                else
                {
                    var body = await resp.Content.ReadAsStringAsync();
                    string errorMsg = $"Error calling /issue/{ftQueueID}/{ftQueueItemID}/delivered: {resp.StatusCode} - {resp.ReasonPhrase} - {body}";
                    return (false, null!, errorMsg);
                }
            }
        }
    }
}