using System;
using System.Threading.Tasks;

namespace LightBDD.Testing.Http
{
    public interface IMockHttpResponseBuilder
    {
        IMockHttpHandlerConfigurator RespondAsync(Func<ITestableHttpRequest, IMockHttpResponse, Task> response);
    }
}