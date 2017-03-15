using System;
using System.Dynamic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LightBDD.Testing.Http
{
    public class TestableHttpClient : IDisposable
    {
        private readonly HttpClient _client = new HttpClient();
        private TestableHttpResponseMessage _lastResponse;

        public TestableHttpClient(Uri baseUri)
        {
            if (baseUri == null)
                throw new ArgumentNullException(nameof(baseUri));

            _client.BaseAddress = baseUri;
        }

        public HttpRequestHeaders DefaultHeaders => _client.DefaultRequestHeaders;

        public TestableHttpResponseMessage LastResponse => _lastResponse;

        public async Task<TestableHttpResponseMessage> SendAsync(HttpRequestMessage request)
        {
            return _lastResponse = await TestableHttpResponseMessage.FromResponseAsync(await _client.SendAsync(request, HttpCompletionOption.ResponseContentRead));
        }
        public Task<TestableHttpResponseMessage> GetAsync(string uri)
            => SendAsync(HttpMethod.Get, uri);

        public Task<TestableHttpResponseMessage> SendAsync(HttpMethod method, string uri)
            => SendAsync(new HttpRequestMessage { Method = method, RequestUri = new Uri(uri, UriKind.RelativeOrAbsolute) });

        public Task<TestableHttpResponseMessage> SendStringAsync(HttpMethod method, string uri, string content, Encoding encoding, string mediaType)
            => SendAsync(new HttpRequestMessage { Method = method, RequestUri = new Uri(uri, UriKind.RelativeOrAbsolute), Content = new StringContent(content, encoding, mediaType) });

        public Task<TestableHttpResponseMessage> SendJsonStringAsync(HttpMethod method, string uri, string content)
            => SendStringAsync(method, uri, content, Encoding.UTF8, "application/json");

        public Task<TestableHttpResponseMessage> SendJsonAsync(HttpMethod method, string uri, object content, JsonSerializerSettings settings = null)
            => SendJsonStringAsync(method, uri, JsonConvert.SerializeObject(content, settings));

        public void Dispose()
        {
            _lastResponse = null;
            _client.Dispose();
        }
    }

    public class TestableHttpResponseMessage
    {
        private readonly string _content;

        private TestableHttpResponseMessage(HttpResponseMessage response, string content)
        {
            OriginalResponse = response;
            _content = content;
        }

        public static async Task<TestableHttpResponseMessage> FromResponseAsync(HttpResponseMessage response)
        {
            return new TestableHttpResponseMessage(response, await response.Content.ReadAsStringAsync());
        }

        public TestableHttpResponseMessage EnsureSuccessStatusCode()
        {
            OriginalResponse.EnsureSuccessStatusCode();
            return this;
        }

        public HttpResponseHeaders Headers => OriginalResponse.Headers;
        public string ReasonPhrase => OriginalResponse.ReasonPhrase;
        public HttpStatusCode StatusCode => OriginalResponse.StatusCode;
        public HttpResponseMessage OriginalResponse { get; }

        public T ToJson<T>(JsonSerializerSettings settings = null) => JsonConvert.DeserializeObject<T>(_content, settings);
        public dynamic ToJson(JsonSerializerSettings settings = null) => JsonConvert.DeserializeObject<ExpandoObject>(_content, settings);
        public object ToAnonymousJson(Type expectedModel, JsonSerializerSettings settings = null) => JsonConvert.DeserializeAnonymousType(_content, expectedModel, settings);
    }
}
