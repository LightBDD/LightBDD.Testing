using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LightBDD.Framework.Formatting;
using LightBDD.Testing.Http;
using LightBDD.Testing.Tests.Helpers;
using LightBDD.XUnit2;
using Newtonsoft.Json;
using Xunit;

namespace LightBDD.Testing.Tests.Acceptance
{
    public partial class MockHttpServer_feature : FeatureFixture, IDisposable
    {
        private HttpResponseMessage _response;
        private MockHttpServer _server;

        public MockHttpServer_feature(Xunit.Abstractions.ITestOutputHelper output) : base(output)
        {
        }

        public void Dispose()
        {
            _server?.Dispose();
        }

        private void Given_mock_http_server()
        {
            _server = MockHttpServer.Start(MockHttpServerHelper.GetNextPort(), cfg => cfg);
        }

        private void Given_server_configured_for_METHOD_URL_and_body_to_return_status_code(HttpMethod method, string url, string body, HttpStatusCode code)
        {
            _server.Reconfigure(false, cfg =>
                cfg.ForRequest(req => req.Method == method && req.RelativeUri == url && req.GetContentAsString() == body)
                    .RespondStatusCode(code)
                    .Apply());
        }

        private void Given_server_configured_for_METHOD_URL_and_json_body_to_return_status_code<T>(HttpMethod method, string url, Expression<Func<T, bool>> body, HttpStatusCode code)
        {
            _server.Reconfigure(false, cfg =>
                cfg.ForRequest(
                        req =>
                            req.Method == method && req.RelativeUri == url &&
                            body.Compile().Invoke(req.GetContentAsJson<T>()))
                    .RespondStatusCode(code)
                    .Apply());
        }

        private void Given_server_configured_for_METHOD_URL_to_return_status_code(HttpMethod method, string url, HttpStatusCode code)
        {
            _server.Reconfigure(false, cfg => cfg.ForRequest(method, url).RespondStatusCode(code).Apply());
        }

        private void Given_server_configured_for_METHOD_URL_to_return_status_code_with_content(HttpMethod method, string url, HttpStatusCode code, string content)
        {
            _server.Reconfigure(false, cfg =>
                cfg.ForRequest(method, url)
                    .RespondStringContent(code, content, Encoding.UTF8, "text/plain")
                    .Apply());
        }

        private void Given_server_configured_for_METHOD_URL_to_return_status_code_with_json_content(HttpMethod method, string url, HttpStatusCode code, object content)
        {
            _server.Reconfigure(false, cfg =>
                cfg.ForRequest(method, url)
                    .RespondJsonContent(code, content)
                    .Apply());
        }

        private async Task<HttpResponseMessage> PerformCallAsync(HttpMethod method, string url, HttpContent content = null)
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(method, _server.BaseAddress + url) { Content = content };
                return await client.SendAsync(request).ConfigureAwait(false);
            }
        }

        private async void Then_server_should_allow_reconfiguration_under_load()
        {
            var items = Enumerable.Range(0, 1000).ToDictionary(i => Guid.NewGuid(), i => new List<HttpStatusCode>());
            var expectedCodes = new[] { HttpStatusCode.OK, HttpStatusCode.Accepted, HttpStatusCode.BadRequest };

            Func<Guid, Task> operation = async id =>
            {
                var path = $"/items/{id}";
                foreach (var expectedCode in expectedCodes)
                {
                    _server.Reconfigure(false,
                        c => c.ForRequest(HttpMethod.Get, path).RespondStatusCode(expectedCode).Apply());
                    items[id].Add((await PerformCallAsync(HttpMethod.Get, path)).StatusCode);
                }
            };

            await Task.WhenAll(items.Select(i => Task.Run(() => operation(i.Key))));

            Assert.Equal(items.Count, items.Count(i => i.Value.SequenceEqual(expectedCodes)));
        }

        private async void Then_the_response_should_have_content(string content)
        {
            Assert.Equal(content, await _response.Content.ReadAsStringAsync());
        }

        private void Then_the_response_should_have_status_code(HttpStatusCode code)
        {
            Assert.Equal(code, _response.StatusCode);
        }

        private async void When_client_performs_METHOD_URL_request(HttpMethod method, string url)
        {
            _response = await PerformCallAsync(method, url);
        }

        private async void When_client_performs_METHOD_URL_request_with_content(HttpMethod method, string url, string content)
        {
            _response = await PerformCallAsync(method, url, new StringContent(content));
        }

        private async void When_client_performs_METHOD_URL_request_with_json_content(HttpMethod method, string url, [FormatJson] object content)
        {
            _response = await PerformCallAsync(method, url, new StringContent(JsonConvert.SerializeObject(content)));
        }

        private void When_server_reconfigures_METHOD_URL_to_return_status_code_and_DISCARD_others(HttpMethod method, string url, HttpStatusCode code, [FormatBoolean("discard", "preserve")] bool discard)
        {
            _server.Reconfigure(discard, cfg => cfg.ForRequest(method, url).RespondStatusCode(code).Apply());
        }
    }
}