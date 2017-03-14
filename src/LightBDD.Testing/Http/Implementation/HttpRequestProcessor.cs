using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LightBDD.Testing.Http.Implementation
{
    internal class HttpRequestProcessor
    {
        private readonly Func<HttpRequest, bool> _predicate;
        private readonly Func<HttpRequest, HttpResponse, Task> _processor;

        public HttpRequestProcessor(Func<HttpRequest, bool> predicate, Func<HttpRequest, HttpResponse, Task> processor)
        {
            _predicate = predicate;
            _processor = processor;
        }

        public bool Match(HttpRequest request)
        {
            try
            {
                return _predicate(request);
            }
            catch (Exception e)
            {
                Trace.TraceError($"HttpRequest match failed: {e}");
                return false;
            }
        }

        public Task ProcessRequestAsync(HttpRequest request, HttpResponse response) => _processor(request, response);
    }
}