using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using LightBDD.Testing.Http.Implementation;

namespace LightBDD.Testing.Http
{
    public class MockHttpServerConfigurator
    {
        private readonly List<MockHttpRequestProcessor> _processors = new List<MockHttpRequestProcessor>();
        internal IEnumerable<MockHttpRequestProcessor> Processors => _processors;

        public IMockHttpResponseBuilder ForRequest(HttpMethod method, string relativeUri)
            => ForRequest(method, u => string.Equals(u, relativeUri, StringComparison.OrdinalIgnoreCase));
        public IMockHttpResponseBuilder ForRequest(HttpMethod method, Func<string, bool> relativeUriMatch)
            => ForRequest(r => r.Method.Equals(method) && relativeUriMatch(r.RelativeUri));

        public IMockHttpResponseBuilder ForRequest(Func<MockHttpRequest, bool> predicate)
        {
            return new MockHttpHandlerBuilder(this, predicate);
        }

        internal MockHttpServerConfigurator Add(Func<MockHttpRequest, bool> predicate, Func<MockHttpRequest, MockHttpResponse, Task> response)
        {
            _processors.Add(new MockHttpRequestProcessor(predicate, response));
            return this;
        }
    }
}