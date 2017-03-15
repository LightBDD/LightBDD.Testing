using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace LightBDD.Testing.Http.Implementation
{
    internal class TestableHttpResponseMessage : ITestableHttpResponseMessage
    {
        public TestableHttpResponseMessage(HttpResponseMessage response, string content)
        {
            OriginalResponse = response;
            Content = content;
        }

        public string Content { get; }
        public HttpResponseHeaders Headers => OriginalResponse.Headers;
        public string ReasonPhrase => OriginalResponse.ReasonPhrase;
        public HttpStatusCode StatusCode => OriginalResponse.StatusCode;
        public HttpResponseMessage OriginalResponse { get; }
    }
}