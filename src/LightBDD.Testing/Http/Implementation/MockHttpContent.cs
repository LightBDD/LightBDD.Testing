using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace LightBDD.Testing.Http.Implementation
{
    internal class MockHttpContent : ITestableHttpContent
    {
        public MockHttpContent(byte[] content, Encoding contentEncoding, string contentType, IReadOnlyDictionary<string, string> headers)
        {
            Content = content;
            ContentEncoding = contentEncoding;
            ContentType = contentType;
            Headers = headers;
        }

        public MockHttpContent(HttpContent httpContent, byte[] content)
        {
            Headers = httpContent.Headers.ToDictionary(h => h.Key, h => h.Value.FirstOrDefault());
            ContentEncoding = httpContent.Headers.ContentEncoding.Select(Encoding.GetEncoding).FirstOrDefault() ?? Encoding.ASCII;
            ContentType = httpContent.Headers.ContentType?.MediaType;
            Content = content;
        }

        public Encoding ContentEncoding { get; }
        public IReadOnlyDictionary<string, string> Headers { get; }
        public byte[] Content { get; }
        public string ContentType { get; }
        public long ContentLength => Content?.Length ?? -1;
    }
}