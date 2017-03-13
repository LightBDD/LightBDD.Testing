using System;
using System.Diagnostics;

namespace LightBDD.Testing.Http.Implementation
{
    internal class HttpRequestProcessor
    {
        private readonly Func<HttpRequest, bool> _predicate;
        private readonly Action<HttpRequest, HttpResponse> _processor;

        public HttpRequestProcessor(Func<HttpRequest, bool> predicate, Action<HttpRequest, HttpResponse> processor)
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

        public void ProcessRequest(HttpRequest request, HttpResponse response) => _processor(request, response);
    }
}