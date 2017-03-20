using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace LightBDD.Testing.Http.Implementation
{
    internal class TestableHttpResponse : ITestableHttpResponse
    {
        public TestableHttpResponse(HttpResponseMessage response, byte[] content, ITestableHttpRequest request)
        {
            Content = new MockHttpContent(response.Content, content);
            Request = request;
            ReasonPhrase = response.ReasonPhrase;
            Headers = response.Headers.ToDictionary(h => h.Key, h => h.Value.FirstOrDefault());
            StatusCode = response.StatusCode;
        }

        public string ReasonPhrase { get; }
        public HttpStatusCode StatusCode { get; }

        public override string ToString()
        {
            return $"[StatusCode: {StatusCode}, ContentLength: {Content.ContentLength}]";
        }

        public ITestableHttpContent Content { get; }
        public ITestableHttpRequest Request { get; }
        public IReadOnlyDictionary<string, string> Headers { get; }
    }
}