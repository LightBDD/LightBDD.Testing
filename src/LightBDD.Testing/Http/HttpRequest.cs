using System;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Text;

namespace LightBDD.Testing.Http
{
    public class HttpRequest
    {
        private readonly HttpListenerRequest _request;

        internal HttpRequest(HttpListenerRequest request, byte[] content, Uri baseAddress)
        {
            Content = content;
            _request = request;
            RelativeUri = "/" + baseAddress.MakeRelativeUri(_request.Url);
        }

        public byte[] Content { get; }
        public string RelativeUri { get; }
        public Uri Uri => _request.Url;
        public NameValueCollection Headers => _request.Headers;
        public Encoding ContentEncoding => _request.ContentEncoding;
        public string ContentType => _request.ContentType;
        public HttpMethod Method => new HttpMethod(_request.HttpMethod);
    }
}