using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LightBDD.Testing.Http.Implementation
{
    internal class MockHttpRequestProcessor
    {
        private readonly Func<MockHttpRequest, bool> _predicate;
        private readonly Func<MockHttpRequest, MockHttpResponse, Task> _processor;

        public MockHttpRequestProcessor(Func<MockHttpRequest, bool> predicate, Func<MockHttpRequest, MockHttpResponse, Task> processor)
        {
            _predicate = predicate;
            _processor = processor;
        }

        public bool Match(MockHttpRequest request)
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

        public Task ProcessRequestAsync(MockHttpRequest request, MockHttpResponse response) => _processor(request, response);
    }
}