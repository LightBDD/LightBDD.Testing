using System.Collections.Generic;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace LightBDD.Testing.Http
{
    public interface IMockHttpResponse
    {
        IMockHttpResponse SetStatusCode(HttpStatusCode statusCode);
        IMockHttpResponse SetContent(byte[] content, Encoding encoding, string contentType);
        IMockHttpResponse SetStringContent(string content, Encoding encoding, string contentType);
        IMockHttpResponse SetJsonContent(object content, JsonSerializerSettings settings = null);
        IMockHttpResponse SetHeaders(IEnumerable<KeyValuePair<string, string>> responseHeaders);
    }
}