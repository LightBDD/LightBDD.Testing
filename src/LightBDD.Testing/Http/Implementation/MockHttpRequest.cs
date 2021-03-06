using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace LightBDD.Testing.Http.Implementation
{
    internal class MockHttpRequest : ITestableHttpRequest
    {
        internal MockHttpRequest(HttpListenerRequest request, byte[] content, Uri baseAddress)
        {
            Uri = request.Url;
            Method = new HttpMethod(request.HttpMethod);
            RelativeUri = "/" + baseAddress.MakeRelativeUri(request.Url);
            Headers = request.Headers.AllKeys.ToDictionary(key => key, key => request.Headers[key]);
            Content = new MockHttpContent(content, request.ContentEncoding, request.ContentType, new Dictionary<string, string>());
            BaseUri = baseAddress;
        }

        public MockHttpRequest(HttpRequestMessage request, byte[] content, Uri baseAddress)
        {
            Uri = new Uri(baseAddress, request.RequestUri);
            Method = request.Method;
            RelativeUri = request.RequestUri.ToString();
            Headers = request.Headers.ToDictionary(h => h.Key, h => h.Value.FirstOrDefault());
            Content = request.Content != null ? new MockHttpContent(request.Content, content) : null;
            BaseUri = baseAddress;
        }

        public MockHttpRequest(Uri baseUri, HttpMethod method, string relativeUri, ITestableHttpContent content, Dictionary<string, string> headers)
        {
            BaseUri = baseUri;
            Method = method;
            RelativeUri = relativeUri;
            Content = content;
            Headers = headers;
        }

        public string RelativeUri { get; }
        public Uri Uri { get; }
        public HttpMethod Method { get; }
        public IReadOnlyDictionary<string, string> Headers { get; }
        public ITestableHttpContent Content { get; }
        public Uri BaseUri { get; }
    }
}