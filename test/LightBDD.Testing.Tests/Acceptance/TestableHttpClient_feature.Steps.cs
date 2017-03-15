using System.Net;
using System.Net.Http;
using LightBDD.Testing.Http;
using LightBDD.Testing.Tests.Helpers;
using LightBDD.XUnit2;
using Xunit;

namespace LightBDD.Testing.Tests.Acceptance
{
    public partial class TestableHttpClient_feature : FeatureFixture
    {
        private void Then_response_should_contain_status_code_and_json_content(HttpStatusCode code, object content)
        {
            Assert.NotNull(_client.LastResponse);
            Assert.Equal(code, _client.LastResponse.StatusCode);
            Assert.Equal(content, _client.LastResponse.ToAnonymousJson(content.GetType()));
        }

        private MockHttpServer _server;
        private TestableHttpClient _client;

        public TestableHttpClient_feature(Xunit.Abstractions.ITestOutputHelper output) : base(output)
        {
        }

        private async void When_client_sends_METHOD_URL_request(HttpMethod method, string url)
        {
            await _client.SendAsync(method, url);
        }

        private void Given_test_http_client()
        {
            _client = new TestableHttpClient(_server.BaseAddress);
        }

        private void Given_mock_http_server()
        {
            _server = MockHttpServer.Start(MockHttpServerHelper.GetNextPort(), cfg => cfg);
        }

        private void Given_server_configured_for_METHOD_URL_to_return_status_code_with_json_content(HttpMethod method, string url, HttpStatusCode code, object content)
        {
            _server.Reconfigure(false, cfg =>
                cfg.ForRequest(method, url)
                    .RespondJsonContent(code, content)
                    .Apply());
        }
    }
}