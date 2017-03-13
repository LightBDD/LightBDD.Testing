using System.Net;

namespace LightBDD.Testing.Http
{
    public class HttpResponse
    {
        private readonly HttpListenerResponse _response;

        internal HttpResponse(HttpListenerResponse response)
        {
            _response = response;
        }

        public HttpResponse SetStatusCode(HttpStatusCode statusCode)
        {
            _response.StatusCode = (int)statusCode;
            return this;
        }
    }
}