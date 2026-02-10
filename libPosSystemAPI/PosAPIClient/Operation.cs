using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using fiskaltrust.DevKit.POSSystemAPI.lib.DTO;
using fiskaltrust.DevKit.POSSystemAPI.lib.PosAPIUtils;

namespace fiskaltrust.DevKit.POSSystemAPI.lib.PosAPIClient
{
    public static class JsonConfiguration
    {
        public static readonly JsonSerializerOptions DefaultOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        };
    }

    /// <summary>
    /// Wraps an operation request and response with its unique operation ID.
    /// An operation must be executed via Execute() before the response can be accessed.
    /// </summary>
    public class Operation
    {
        public Guid OperationID { get; private set; }
        public HttpRequestMessage Request { get; private set; }
        /// <summary>
        /// Returns the response. Might be null if the operation was not executed OR the execution failed.
        /// </summary>
        public HttpResponseMessage? Response { get; private set; }

        internal Operation(HttpRequestMessage req)
        {
            req.Headers.TryGetValues("x-operation-id", out var values);
            if (values != null && values.Any())
            {
                OperationID = Guid.Parse(values.First());
            }
            else
            {
                throw new Exception("Operation ID not found in request headers.");
            }

            Request = req;
        }

        internal async Task Execute()
        {
            if (ftPosAPI.Client == null) throw new InvalidOperationException("ftPosAPI is not initialized. Call ftPosAPI.Init() first.");

            HttpResponseMessage resp = await ftPosAPI.Client.SendAsync(Request);
            Response = resp;
        }

        public bool IsSuccess { get => Response != null && Response.IsSuccessStatusCode; }

        internal async Task<string> GetResponseContentAsync()
        {
            if (Response == null)
            {
                throw new InvalidOperationException("Response is not set. Execute the operation first.");
            }
            if (!IsSuccess)
            {
                throw new InvalidOperationException("Operation was not successful.");
            }
            return await Response.Content.ReadAsStringAsync();
        }
    }

    /// <summary>
    /// A generic version of Operation where a response type T is given to simplify response handling.
    /// </summary>
    public class Operation<T> : Operation where T : class
    {
        internal Operation(HttpRequestMessage req) : base(req)
        {
        }

        /// <summary>
        /// Gets the response deserialized as T if the operation was successful.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">If the operation was not successful or deserialization fails.</exception>
        public async Task<T> GetResponseAsAsync()
        {
            if (!Response!.IsSuccessStatusCode) throw new InvalidOperationException("Operation was not successful.");

            var content = await GetResponseContentAsync();
            try
            {
                T? response = System.Text.Json.JsonSerializer.Deserialize<T>(content, JsonConfiguration.DefaultOptions);
                if (response == null) throw new Exception($"Deserialization of {typeof(T).Name} failed; original: {content}");
                return response;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Deserialization of {typeof(T).Name} failed; original: {content}", ex);
            }
        }

        /// <summary>
        /// Gets the error response if the operation was not successful.
        /// </summary>
        /// <exception cref="InvalidOperationException">If the response is not set or the operation was successful.</exception>
        public async Task<ErrorResponse?> GetResponseErrorAsync()
        {
            if (IsSuccess) throw new InvalidOperationException("Operation was successful, no error to retrieve.");
            if (Response == null) throw new InvalidOperationException("Response is not set. Execute the operation first.");

            try
            {
                return await Response.Content.ReadFromJsonAsync<ErrorResponse>();
            }
            catch (Exception ex)
            {
                Logger.LogWarning("Failed to read response error body; setting error title from fully response body: " + ex.Message);
                var respBody = await Response!.Content.ReadAsStringAsync();
                return new ErrorResponse()
                {
                    Title = respBody,
                    Status = 0
                };
            }
        }
    }
}
