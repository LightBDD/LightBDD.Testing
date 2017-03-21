using System;
using System.Collections.Generic;
using System.Net.Http;

namespace LightBDD.Testing.Http
{
    public interface ITestableHttpRequest
    {
        string RelativeUri { get; }
        Uri BaseUri { get; }
        Uri Uri { get; }
        IReadOnlyDictionary<string, string> Headers { get; }
        HttpMethod Method { get; }
        ITestableHttpContent Content { get; }
    }
}