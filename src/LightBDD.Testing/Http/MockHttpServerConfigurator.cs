using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using LightBDD.Testing.Http.Implementation;

namespace LightBDD.Testing.Http
{
    public class MockHttpServerConfigurator
    {
        private readonly List<HttpRequestProcessor> _processors = new List<HttpRequestProcessor>();
        internal IEnumerable<HttpRequestProcessor> Processors => _processors;

        public IHttpResponseBuilder ForRequest(HttpMethod method, string relativeUri)
            => ForRequest(method, u => string.Equals(u, relativeUri, StringComparison.OrdinalIgnoreCase));
        public IHttpResponseBuilder ForRequest(HttpMethod method, Func<string, bool> relativeUriMatch)
            => ForRequest(r => r.Method.Equals(method) && relativeUriMatch(r.RelativeUri));

        public IHttpResponseBuilder ForRequest(Func<HttpRequest, bool> predicate)
        {
            return new HttpHandlerBuilder(this, predicate);
        }

        internal MockHttpServerConfigurator Add(Func<HttpRequest, bool> predicate, Func<HttpRequest, HttpResponse, Task> response)
        {
            _processors.Add(new HttpRequestProcessor(predicate, response));
            return this;
        }
    }
}