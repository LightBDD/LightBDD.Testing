using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace LightBDD.Testing.Http
{
    public interface ITestableHttpResponse
    {
        string Content { get; }
        HttpResponseHeaders Headers { get; }
        string ReasonPhrase { get; }
        HttpStatusCode StatusCode { get; }
        HttpResponseMessage OriginalResponse { get; }
    }
}