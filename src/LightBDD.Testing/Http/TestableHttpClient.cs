using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using LightBDD.Testing.Http.Implementation;
using Newtonsoft.Json;

namespace LightBDD.Testing.Http
{
    public class TestableHttpClient : IDisposable
    {
        private readonly HttpClient _client = new HttpClient();

        public TestableHttpClient(Uri baseUri)
        {
            if (baseUri == null)
                throw new ArgumentNullException(nameof(baseUri));
            _client.BaseAddress = baseUri;
        }

        public HttpRequestHeaders DefaultHeaders => _client.DefaultRequestHeaders;

        public ITestableHttpResponse LastResponse { get; private set; } = new NoTestableHttpResponse();

        public async Task<ITestableHttpResponse> SendAsync(HttpRequestMessage request)
        {
            return LastResponse = await TestableHttpResponses.FromResponseAsync(await _client.SendAsync(request, HttpCompletionOption.ResponseContentRead));
        }
        public Task<ITestableHttpResponse> GetAsync(string uri)
            => SendAsync(HttpMethod.Get, uri);

        public Task<ITestableHttpResponse> SendAsync(HttpMethod method, string uri)
            => SendAsync(new HttpRequestMessage { Method = method, RequestUri = new Uri(uri, UriKind.RelativeOrAbsolute) });

        public Task<ITestableHttpResponse> SendStringAsync(HttpMethod method, string uri, string content, Encoding encoding, string mediaType)
            => SendAsync(new HttpRequestMessage { Method = method, RequestUri = new Uri(uri, UriKind.RelativeOrAbsolute), Content = new StringContent(content, encoding, mediaType) });

        public Task<ITestableHttpResponse> SendJsonStringAsync(HttpMethod method, string uri, string content)
            => SendStringAsync(method, uri, content, Encoding.UTF8, "application/json");

        public Task<ITestableHttpResponse> SendJsonAsync(HttpMethod method, string uri, object content, JsonSerializerSettings settings = null)
            => SendJsonStringAsync(method, uri, JsonConvert.SerializeObject(content, settings));

        public void Dispose()
        {
            LastResponse = null;
            _client.Dispose();
        }
    }
}