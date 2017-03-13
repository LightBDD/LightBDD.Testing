using System;

namespace LightBDD.Testing.Http.Implementation
{
    internal class HttpHandlerBuilder : IHttpResponseBuilder, IHttpHandlerConfigurator
    {
        private readonly MockHttpServerConfigurator _configurator;
        private readonly Func<HttpRequest, bool> _predicate;
        private Action<HttpRequest, HttpResponse> _response;

        public HttpHandlerBuilder(MockHttpServerConfigurator configurator, Func<HttpRequest, bool> predicate)
        {
            _configurator = configurator;
            _predicate = predicate;
        }

        public IHttpHandlerConfigurator Respond(Action<HttpRequest, HttpResponse> response)
        {
            _response = response;
            return this;
        }

        public MockHttpServerConfigurator Apply()
        {
            return _configurator.Add(_predicate, _response);
        }
    }
}