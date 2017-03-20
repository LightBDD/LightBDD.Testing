using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LightBDD.Testing.Http.Implementation
{
    internal class MockHttpRequestProcessor
    {
        private readonly Func<ITestableHttpRequest, bool> _predicate;
        private readonly Func<ITestableHttpRequest, MockHttpResponse, Task> _processor;

        public MockHttpRequestProcessor(Func<ITestableHttpRequest, bool> predicate, Func<ITestableHttpRequest, MockHttpResponse, Task> processor)
        {
            _predicate = predicate;
            _processor = processor;
        }

        public bool Match(ITestableHttpRequest request)
        {
            try
            {
                return _predicate(request);
            }
            catch (Exception e)
            {
                Trace.TraceError($"MockHttpRequest match failed: {e}");
                return false;
            }
        }

        public Task ProcessRequestAsync(ITestableHttpRequest request, MockHttpResponse response) => _processor(request, response);
    }
}