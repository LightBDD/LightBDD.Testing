using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using LightBDD.Framework.Formatting;
using LightBDD.Testing.Http;
using LightBDD.Testing.Tests.Helpers;
using LightBDD.XUnit2;
using Xunit;
using Xunit.Abstractions;

namespace LightBDD.Testing.Tests.Acceptance
{
    public partial class RecordingHttpProxy_feature : FeatureFixture
    {
        private MockHttpServer _server;
        private RecordingHttpProxy _proxy;
        private TestableHttpClient _client;
        private string _recordingsRepositoryPath = AppContext.BaseDirectory + "\\recordings\\" + Guid.NewGuid();

        public RecordingHttpProxy_feature(ITestOutputHelper output) : base(output)
        {
        }

        private void Given_target_mock_http_server()
        {
            _server = MockHttpServer.Start(MockHttpServerHelper.GetNextPort());
        }

        private void Given_recording_proxy_for_target_server()
        {
            _proxy = new RecordingHttpProxy(MockHttpServerHelper.GetNextPort(), _server.BaseAddress, RecordingHttpProxy.Mode.Record, new RecordedHttpCallRepository(_recordingsRepositoryPath));
        }

        private void Given_test_http_client_pointing_to_the_proxy()
        {
            _client = new TestableHttpClient(_proxy.BaseAddress);
        }

        private void Given_server_configured_for_METHOD_URL_with_headers_REQUESTHEADERS_and_json_content_REQUESTCONTENT_to_return_status_code_with_headers_RESPONSEHEADERS_and_json_content_RESPONSECONTENT<TRequestContent, TResponseContent>(
            HttpMethod method, string url,
            [FormatCollection]Dictionary<string, string> requestHeaders, [FormatJson]TRequestContent requestContent,
            HttpStatusCode code, [FormatCollection]Dictionary<string, string> responseHeaders, [FormatJson]TResponseContent responseContent)
        {
            _server.Reconfigure(false, cfg =>
                cfg.ForRequest(req =>
                        req.Method == method && req.RelativeUri == url &&
                        requestHeaders.All(h => req.Headers.ContainsKey(h.Key) && req.Headers[h.Key] == h.Value) &&
                        Equals(req.GetContentAsAnonymousJson(requestContent), requestContent))
                    .Respond(rsp => rsp.SetStatusCode(code).SetHeaders(responseHeaders).SetJsonContent(responseContent))
                    .Apply());
        }
        private void Given_server_configured_for_METHOD_URL_to_return_status_code(HttpMethod method, string url, HttpStatusCode code)
        {
            _server.Reconfigure(false, cfg => cfg.ForRequest(method, url).RespondStatusCode(code).Apply());
        }

        private async void When_client_sends_METHOD_URL_request_with_headers_and_json_content(HttpMethod method, string url, [FormatCollection]Dictionary<string, string> headers, object content)
        {
            await _client.SendAsync(new HttpRequestMessage(method, url).WithJsonContent(content).WithHeaders(headers));
        }

        private void Then_response_should_contain_status_code_with_headers_and_json_content<T>(HttpStatusCode code, [FormatCollection]Dictionary<string, string> headers, T content)
        {
            Assert.NotNull(_client.LastResponse);
            Assert.Equal(code, _client.LastResponse.StatusCode);
            Assert.Equal(content, _client.LastResponse.ToAnonymousJson(content));
            foreach (var header in headers)
                Assert.Equal(header.Value, _client.LastResponse.Headers[header.Key]);
        }

        private void Given_proxy_mode_is_changed_to_replay()
        {
            _proxy.Dispose();
            _proxy = new RecordingHttpProxy(_proxy.BaseAddress.Port, _server.BaseAddress, RecordingHttpProxy.Mode.Replay, new RecordedHttpCallRepository(_recordingsRepositoryPath));
        }
    }
}