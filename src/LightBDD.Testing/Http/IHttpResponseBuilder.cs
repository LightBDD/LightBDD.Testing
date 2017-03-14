using System;
using System.Threading.Tasks;

namespace LightBDD.Testing.Http
{
    public interface IHttpResponseBuilder
    {
        IHttpHandlerConfigurator Respond(Func<HttpRequest, HttpResponse, Task> response);
    }
}