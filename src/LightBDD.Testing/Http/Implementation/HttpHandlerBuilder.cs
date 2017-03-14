using System;
using System.Threading;
using System.Threading.Tasks;

namespace LightBDD.Testing.Http.Implementation
{
    internal class HttpHandlerBuilder : IHttpResponseBuilder, IHttpHandlerConfigurator
    {
        private readonly MockHttpServerConfigurator _configurator;
        private Func<HttpRequest, bool> _predicate;
        private Func<HttpRequest, HttpResponse, Task> _response;

        public HttpHandlerBuilder(MockHttpServerConfigurator configurator, Func<HttpRequest, bool> predicate)
        {
            _configurator = configurator;
            _predicate = predicate;
        }

        public IHttpHandlerConfigurator Respond(Func<HttpRequest, HttpResponse, Task> response)
        {
            _response = response;
            return this;
        }

        public MockHttpServerConfigurator Apply()
        {
            return _configurator.Add(_predicate, _response);
        }

        public IHttpHandlerConfigurator ExpireAfterCallNumber(int maxCallNumber)
        {
            var predicate = _predicate;
            var current = 0;
            _predicate = req => predicate(req) && Interlocked.Increment(ref current) <= maxCallNumber;
            return this;
        }

        public IHttpHandlerConfigurator ExpireAfterTime(TimeSpan maxTime)
        {
            var predicate = _predicate;
            var start = DateTimeOffset.UtcNow;
            _predicate = req => (DateTimeOffset.UtcNow - start < maxTime) && predicate(req);
            return this;
        }
    }
}