using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace LightBDD.Testing.Http
{
    public static class HttpRequestMessageExtensions
    {
        public static HttpRequestMessage WithStringContent(this HttpRequestMessage request, string content, Encoding encoding, string mediaType)
        {
            request.Content = new StringContent(content, encoding, mediaType);
            return request;
        }

        public static HttpRequestMessage WithJsonStringContent(this HttpRequestMessage request, string content)
        {
            return request.WithStringContent(content, Encoding.UTF8, "application/json");
        }

        public static HttpRequestMessage WithJsonContent(this HttpRequestMessage request, object content, JsonSerializerSettings settings = null)
        {
            return request.WithJsonStringContent(JsonConvert.SerializeObject(content, settings));
        }

        public static HttpRequestMessage WithHeaders(this HttpRequestMessage request, IEnumerable<KeyValuePair<string, string>> headers)
        {
            foreach (var header in headers)
                request.Headers.Add(header.Key, header.Value);
            return request;
        }
    }
}