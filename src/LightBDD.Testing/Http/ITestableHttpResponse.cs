using System.Collections.Generic;
using System.Net;

namespace LightBDD.Testing.Http
{
    public interface ITestableHttpResponse
    {
        ITestableHttpContent Content { get; }
        IReadOnlyDictionary<string,string> Headers { get; }
        string ReasonPhrase { get; }
        HttpStatusCode StatusCode { get; }
        ITestableHttpRequest Request { get; }
    }
}