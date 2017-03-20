using System.Linq;
using System.Net.Http;
using System.Text;

namespace LightBDD.Testing.Http.Implementation
{
    internal class MockHttpContent : ITestableHttpContent
    {
        public MockHttpContent(byte[] content, Encoding contentEncoding, string contentType)
        {
            Content = content;
            ContentEncoding = contentEncoding;
            ContentType = contentType;
        }

        public MockHttpContent(HttpContent httpContent, byte[] content)
        {
            ContentEncoding = httpContent.Headers.ContentEncoding.Select(Encoding.GetEncoding).FirstOrDefault()??Encoding.ASCII;
            ContentType = httpContent.Headers.ContentType?.MediaType;
            Content = content;
        }

        public Encoding ContentEncoding { get; }
        public byte[] Content { get; }
        public string ContentType { get; }
        public long ContentLength => Content?.Length ?? -1;
    }
}