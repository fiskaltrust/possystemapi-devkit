using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace fiskaltrust.DevKit.POSSystemAPI.lib.PosAPIClient
{
    public interface IJsonContentConverter
    {
        JsonContent ToJsonContent();
    }

    public class APIRequestBuilder<TReq, TResp>() where TResp : class where TReq : IJsonContentConverter
    {
        private HttpMethod _method = HttpMethod.Get;

        public APIRequestBuilder<TReq, TResp> SetMethod(HttpMethod method)
        {
            _method = method;
            return this;
        }

        private string _path = string.Empty;
        public APIRequestBuilder<TReq, TResp> SetPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path cannot be null or empty.", nameof(path));
            }

            _path = ftPosAPI.PathPrefix + (path.StartsWith('/') ? path : "/" + path);              

            return this;
        }

        private IJsonContentConverter? _content = null;
        public APIRequestBuilder<TReq, TResp> SetContent(TReq req)
        {
            _content = req;
            return this;
        }

        private Guid? _operationID = null;
        /// <summary>
        /// Sets a specific operation ID for the request.
        /// If not called the request will get a new random operation ID.
        /// </summary>
        public APIRequestBuilder<TReq, TResp> SetOperationID(Guid operationID)
        {
            _operationID = operationID;
            return this;
        }


        public Operation<TResp> Build()
        {
            if (string.IsNullOrWhiteSpace(_path)) throw new Exception("Path is not set.");

            var req = new HttpRequestMessage(_method, _path);
            req.Headers.Add("x-operation-id", (_operationID ?? Guid.NewGuid()).ToString());
            if (_content != null)
            {
                req.Content = _content.ToJsonContent();
            }
            
            return new Operation<TResp>(req);
        }
    }
}

