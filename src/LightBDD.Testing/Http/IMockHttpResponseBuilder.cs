using System;
using System.Threading.Tasks;
using LightBDD.Testing.Http.Implementation;

namespace LightBDD.Testing.Http
{
    public interface IMockHttpResponseBuilder
    {
        IMockHttpHandlerConfigurator RespondAsync(Func<ITestableHttpRequest, IMockHttpResponse, Task> response);
    }
}