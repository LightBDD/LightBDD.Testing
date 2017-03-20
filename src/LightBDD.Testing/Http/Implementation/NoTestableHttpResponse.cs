using System;
using System.Collections.Generic;
using System.Net;

namespace LightBDD.Testing.Http.Implementation
{
    internal class NoTestableHttpResponse : ITestableHttpResponse
    {
        public ITestableHttpContent Content { get { throw CreateException(); } }
        public string ReasonPhrase { get { throw CreateException(); } }
        public HttpStatusCode StatusCode { get { throw CreateException(); } }
        public ITestableHttpRequest Request { get { throw CreateException(); } }
        public IReadOnlyDictionary<string, string> Headers { get { throw CreateException(); } }

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
