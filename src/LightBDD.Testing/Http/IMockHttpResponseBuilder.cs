using System;
using System.Threading.Tasks;

namespace LightBDD.Testing.Http
{
    public interface IMockHttpResponseBuilder
    {
        IMockHttpHandlerConfigurator Respond(Func<MockHttpRequest, MockHttpResponse, Task> response);
    }
}