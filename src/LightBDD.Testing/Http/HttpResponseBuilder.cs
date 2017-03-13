using System;

namespace LightBDD.Testing.Http
{
    public interface IHttpResponseBuilder
    {
        IHttpHandlerConfigurator Respond(Action<HttpRequest, HttpResponse> response);
    }
}