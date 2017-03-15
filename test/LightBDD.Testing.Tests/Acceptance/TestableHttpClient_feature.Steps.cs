﻿using System;
using System.Net;
using System.Net.Http;
using LightBDD.Testing.Http;
using LightBDD.Testing.Http.Implementation;
using LightBDD.Testing.Tests.Helpers;
using LightBDD.XUnit2;
using Xunit;

namespace LightBDD.Testing.Tests.Acceptance
{
    public partial class TestableHttpClient_feature : FeatureFixture
    {
        private TestableHttpClient _client;
        private MockHttpServer _server;

        public TestableHttpClient_feature(Xunit.Abstractions.ITestOutputHelper output) : base(output)
        {
        }

        private void Given_mock_http_server()
        {
            _server = MockHttpServer.Start(MockHttpServerHelper.GetNextPort(), cfg => cfg);
        }

        private void Given_server_configured_for_METHOD_URL_and_json_content_to_return_status_code<TContent>(HttpMethod method, string url, TContent content, HttpStatusCode code)
        {
            _server.Reconfigure(false, cfg =>
                 cfg.ForRequest(req => req.Method == method && req.RelativeUri == url && Equals(req.GetContentAsAnonymousJson(content), content))
                 .RespondStatusCode(code)
                 .Apply());
        }

        private void Given_server_configured_for_METHOD_URL_to_return_status_code_with_json_content(HttpMethod method, string url, HttpStatusCode code, object content)
        {
            _server.Reconfigure(false, cfg =>
                cfg.ForRequest(method, url)
                    .RespondJsonContent(code, content)
                    .Apply());
        }

        private void Given_test_http_client()
        {
            _client = new TestableHttpClient(_server.BaseAddress);
        }

        private void Then_attempt_to_use_LastResponse_properties_should_end_with_InvalidOperationException()
        {
            Assert.Throws<InvalidOperationException>(() => _client.LastResponse.Content);
            Assert.Throws<InvalidOperationException>(() => _client.LastResponse.Headers);
            Assert.Throws<InvalidOperationException>(() => _client.LastResponse.OriginalResponse);
            Assert.Throws<InvalidOperationException>(() => _client.LastResponse.ReasonPhrase);
            Assert.Throws<InvalidOperationException>(() => _client.LastResponse.StatusCode);
        }

        private void Then_LastResponse_should_be_not_null_but_also_not_valid()
        {
            Assert.NotNull(_client.LastResponse);
            Assert.False(_client.LastResponse.IsValidResponse());
        }

        private void Then_response_should_contain_status_code(HttpStatusCode code)
        {
            _client.LastResponse.EnsureStatusCode(code);
        }

        private void Then_response_should_contain_status_code_and_json_content<T>(HttpStatusCode code, T content)
        {
            Assert.NotNull(_client.LastResponse);
            Assert.Equal(code, _client.LastResponse.StatusCode);
            Assert.Equal(content, _client.LastResponse.ToAnonymousJson(content));
        }

        private async void When_client_sends_METHOD_URL_request(HttpMethod method, string url)
        {
            await _client.SendAsync(method, url);
        }

        private async void When_client_sends_METHOD_URL_request_with_json_content(HttpMethod method, string url, object content)
        {
            await _client.SendJsonAsync(method, url, content);
        }
    }
}