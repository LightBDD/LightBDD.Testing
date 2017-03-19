using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LightBDD.Testing.Http
{
    public class MockHttpResponse
    {
        private readonly HttpListenerResponse _response;
        private byte[] _content;

        internal MockHttpResponse(HttpListenerResponse response)
        {
            _response = response;
        }

        public MockHttpResponse SetStatusCode(HttpStatusCode statusCode)
        {
            _response.StatusCode = (int)statusCode;
            return this;
        }

        public MockHttpResponse SetContent(byte[] content, Encoding encoding, string contentType)
        {
            _response.ContentLength64 = content.Length;
            _response.ContentEncoding = encoding;
            _response.ContentType = contentType;
            _content = content;
            return this;
        }

        public MockHttpResponse SetStringContent(string content, Encoding encoding, string contentType)
            => SetContent(encoding.GetBytes(content), encoding, contentType);

        public MockHttpResponse SetJsonContent(object content, JsonSerializerSettings settings = null)
            => SetStringContent(JsonConvert.SerializeObject(content, settings), Encoding.UTF8, "application/json");

        internal void Close()
        {
            _response.Close();
        }

        internal Task SendResponseAsync()
        {
            return _content != null
                ? _response.OutputStream.WriteAsync(_content, 0, _content.Length)
                : Task.CompletedTask;
        }

        public MockHttpResponse SetHeaders(IEnumerable<KeyValuePair<string, string>> responseHeaders)
        {
            foreach (var responseHeader in responseHeaders)
                _response.Headers.Set(responseHeader.Key, responseHeader.Value);
            return this;
        }
    }
}