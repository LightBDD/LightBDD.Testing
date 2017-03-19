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
        public Repeat Repeater { get; private set; } = new Repeat();

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

        public async Task<ITestableHttpResponse> GetAsync(string uri)
            => await SendAsync(HttpMethod.Get, uri);

        public async Task<ITestableHttpResponse> GetUntilAsync(string uri, Func<ITestableHttpResponse, bool> successCondition, string timeoutMessage)
            => await SendUntilAsync(() => new HttpRequestMessage(HttpMethod.Get, uri), successCondition, timeoutMessage);

        public async Task<ITestableHttpResponse> SendUntilAsync(Func<HttpRequestMessage> requestCreator, Func<ITestableHttpResponse, bool> successCondition, string timeoutMessage)
        {
            try
            {
                return await Repeater.RepeatUntilAsync(() => SendAsync(requestCreator()), successCondition, timeoutMessage);
            }
            catch (RepeatTimeoutException<ITestableHttpResponse> exception)
            {
                throw new TestableHttpResponseException(exception.Message, exception.LastValue, exception);
            }
        }

        public async Task<ITestableHttpResponse> SendAsync(HttpMethod method, string uri)
            => await SendAsync(new HttpRequestMessage(method, uri));

        public async Task<ITestableHttpResponse> SendStringAsync(HttpMethod method, string uri, string content, Encoding encoding, string mediaType)
            => await SendAsync(new HttpRequestMessage(method, uri).WithStringContent(content, encoding, mediaType));

        public async Task<ITestableHttpResponse> SendJsonStringAsync(HttpMethod method, string uri, string content)
            => await SendAsync(new HttpRequestMessage(method, uri).WithJsonStringContent(content));

        public async Task<ITestableHttpResponse> SendJsonAsync(HttpMethod method, string uri, object content, JsonSerializerSettings settings = null)
            => await SendAsync(new HttpRequestMessage(method, uri).WithJsonContent(content, settings));

        public void Dispose()
        {
            LastResponse = null;
            _client.Dispose();
        }
    }
}