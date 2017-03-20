using System;
using System.Threading;
using System.Threading.Tasks;

namespace LightBDD.Testing.Http.Implementation
{
    internal class MockHttpHandlerBuilder : IMockHttpResponseBuilder, IMockHttpHandlerConfigurator
    {
        private readonly MockHttpServerConfigurator _configurator;
        private Func<ITestableHttpRequest, bool> _predicate;
        private Func<ITestableHttpRequest, MockHttpResponse, Task> _response;

        public MockHttpHandlerBuilder(MockHttpServerConfigurator configurator, Func<ITestableHttpRequest, bool> predicate)
        {
            _configurator = configurator;
            _predicate = predicate;
        }

        public IMockHttpHandlerConfigurator RespondAsync(Func<ITestableHttpRequest, IMockHttpResponse, Task> response)
        {
            _response = response;
            return this;
        }

        public MockHttpServerConfigurator Apply()
        {
            return _configurator.Add(_predicate, _response);
        }

        public IMockHttpHandlerConfigurator ExpireAfterCallNumber(int maxCallNumber)
        {
            var predicate = _predicate;
            var current = 0;
            _predicate = req => predicate(req) && Interlocked.Increment(ref current) <= maxCallNumber;
            return this;
        }

        public IMockHttpHandlerConfigurator ExpireAfterTime(TimeSpan maxTime)
        {
            var predicate = _predicate;
            var start = DateTimeOffset.UtcNow;
            _predicate = req => (DateTimeOffset.UtcNow - start < maxTime) && predicate(req);
            return this;
        }
    }
}