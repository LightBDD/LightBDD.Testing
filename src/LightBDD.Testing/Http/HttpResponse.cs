using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LightBDD.Testing.Http
{
    public class HttpResponse
    {
        private readonly HttpListenerResponse _response;
        private byte[] _content;

        internal HttpResponse(HttpListenerResponse response)
        {
            _response = response;
        }

        public HttpResponse SetStatusCode(HttpStatusCode statusCode)
        {
            _response.StatusCode = (int)statusCode;
            return this;
        }

        public HttpResponse SetContent(byte[] content, Encoding encoding, string contentType)
        {
            _response.ContentLength64 = content.Length;
            _response.ContentEncoding = encoding;
            _response.ContentType = contentType;
            _content = content;
            return this;
        }

        public HttpResponse SetStringContent(string content, Encoding encoding, string contentType)
            => SetContent(encoding.GetBytes(content), encoding, contentType);

        public HttpResponse SetJsonContent(object content, JsonSerializerSettings settings = null)
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
    }
}