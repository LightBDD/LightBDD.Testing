using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace LightBDD.Testing.Http.Implementation
{
    internal class NoTestableHttpResponse : ITestableHttpResponse
    {
        public string Content { get { throw CreateException(); } }
        public HttpResponseHeaders Headers { get { throw CreateException(); } }
        public string ReasonPhrase { get { throw CreateException(); } }
        public HttpStatusCode StatusCode { get { throw CreateException(); } }
        public HttpResponseMessage OriginalResponse { get { throw CreateException(); } }

        private Exception CreateException()
        {
            throw new InvalidOperationException("No response has been received so far.");
        }

        public override string ToString()
        {
            return "[no response]";
        }
    }
}
