using System;
using System.Net;
using System.Net.Http;
using LightBDD.Framework;
using LightBDD.Framework.Commenting;
using LightBDD.Testing.Http;
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
            Assert.Throws<InvalidOperationException>(() => _client.LastResponse.Request);
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

        private void Then_attempt_to_validate_successful_response_code_should_end_with_exception_containing_actual_response_and_its_content_in_body()
        {
            var exception = Assert.Throws<TestableHttpResponseException>(() => _client.LastResponse.EnsureSuccessStatusCode());
            StepExecution.Current.Comment(exception.Message);
            Assert.NotNull(exception.ActualResponse);
            Assert.Contains(exception.ActualResponse.ToStringContent(), exception.Message);
            Assert.Contains(exception.ResponseLogFile.FullName, exception.Message);
        }

        private void Then_attempt_to_process_expected_successful_body_should_end_with_exception_containing_actual_response_and_its_content_in_body()
        {
            var exception = Assert.Throws<TestableHttpResponseException>(() => _client.LastResponse.ProcessInResponseContext(r => r.ToJson().name));
            StepExecution.Current.Comment(exception.Message);
            Assert.NotNull(exception.ActualResponse);
            Assert.Contains(exception.ActualResponse.ToStringContent(), exception.Message);
        }

        private void Given_server_configured_for_METHOD_URL_to_return_status_code_for_next_NUMBER_of_times(HttpMethod method, string url, HttpStatusCode code, int number)
        {
            _server.Reconfigure(false,
                cfg => cfg.ForRequest(method, url).RespondStatusCode(code).ExpireAfterCallNumber(number).Apply());
        }

        private async void When_client_uses_GetUntilAsync_for_URL_expecting_to_receive_status_code(string url, HttpStatusCode code)
        {
            await _client.GetUntilAsync(url, rsp => rsp.StatusCode == code, "Expected successful response");
        }

        private async void Then_attempt_to_call_GetUntilAsync_for_URL_expecting_to_receive_status_code_should_end_with_response_exception_containing_last_value(string url, HttpStatusCode code)
        {
            _client.Repeater.SetTimeout(TimeSpan.FromSeconds(1));

            var timeoutMessage = "Expected successful code";
            var ex = await Assert.ThrowsAsync<TestableHttpResponseException>(() => _client.GetUntilAsync(url, rsp => rsp.StatusCode == code, timeoutMessage));
            Assert.Contains(timeoutMessage, ex.Message);
            Assert.Contains(ex.ResponseLogFile.FullName, ex.Message);
            Assert.Contains(ex.ActualResponse.ToStringContent(), ex.Message);
        }

        private void Given_server_configured_for_METHOD_URL_to_return_status_code(HttpMethod method, string url, HttpStatusCode code)
        {
            _server.Reconfigure(false,
                cfg => cfg.ForRequest(method, url).RespondStatusCode(code).Apply());
        }
    }
}