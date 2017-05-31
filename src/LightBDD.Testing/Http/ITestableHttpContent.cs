using System.Collections.Generic;
using System.Text;

namespace LightBDD.Testing.Http
{
    public interface ITestableHttpContent
    {
        Encoding ContentEncoding { get; }
        byte[] Content { get; }
        string ContentType { get; }
        long ContentLength { get; }
        IReadOnlyDictionary<string, string> Headers { get; }
    }
}