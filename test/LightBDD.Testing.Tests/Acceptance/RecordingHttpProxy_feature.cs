using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios.Extended;
using LightBDD.XUnit2;

namespace LightBDD.Testing.Tests.Acceptance
{
    [FeatureDescription(
@"In order to execute service tests in isolation but with realistic data,
As a developer,
I want to be able to record real http requests and responses,
and cache them for future tests run")]
    public partial class RecordingHttpProxy_feature
    {
        [Scenario]
        public async Task Proxying_requests_to_target_service_in_recording_mode()
        {
            await Runner.RunScenarioActionsAsync(
                _ => Given_target_mock_http_server(),
                _ => Given_recording_proxy_for_target_server(),
                _ => Given_test_http_client_pointing_to_the_proxy(),

                _ => Given_server_configured_for_METHOD_URL_with_headers_REQUESTHEADERS_and_json_content_REQUESTCONTENT_to_return_status_code_with_headers_RESPONSEHEADERS_and_json_content_RESPONSECONTENT(
                    HttpMethod.Post, "/search", new Dictionary<string, string> { { "CustomHeader", "CustomValue" } }, new { customerId = 123 },
                    HttpStatusCode.OK, new Dictionary<string, string> { { "CustomHeader", "123" } }, new { name = "John", id = "123" }),

                _ => When_client_sends_METHOD_URL_request_with_headers_and_json_content(HttpMethod.Post, "/search", new Dictionary<string, string> { { "CustomHeader", "CustomValue" } }, new { customerId = 123 }),
                _ => Then_response_should_contain_status_code_with_headers_and_json_content(HttpStatusCode.OK, new Dictionary<string, string> { { "CustomHeader", "123" } }, new { name = "John", id = "123" })
                );
        }

        [Scenario]
        public async Task Proxy_should_use_recorded_messages_in_replaying_mode()
        {
            await Runner.RunScenarioActionsAsync(
                _ => Given_target_mock_http_server(),
                _ => Given_recording_proxy_for_target_server(),
                _ => Given_test_http_client_pointing_to_the_proxy(),

                _ => Given_server_configured_for_METHOD_URL_with_headers_REQUESTHEADERS_and_json_content_REQUESTCONTENT_to_return_status_code_with_headers_RESPONSEHEADERS_and_json_content_RESPONSECONTENT(
                    HttpMethod.Post, "/search", new Dictionary<string, string> { { "CustomHeader", "CustomValue" } }, new { customerId = 123 },
                    HttpStatusCode.OK, new Dictionary<string, string> { { "CustomHeader", "123" } }, new { name = "John", id = "123" }),

                _ => When_client_sends_METHOD_URL_request_with_headers_and_json_content(HttpMethod.Post, "/search", new Dictionary<string, string> { { "CustomHeader", "CustomValue" } }, new { customerId = 123 }),
                _ => Then_response_should_contain_status_code_with_headers_and_json_content(HttpStatusCode.OK, new Dictionary<string, string> { { "CustomHeader", "123" } }, new { name = "John", id = "123" }),

                _ => Given_proxy_mode_is_changed_to_replay(),
                _ => Given_server_configured_for_METHOD_URL_to_return_status_code(HttpMethod.Post, "/search", HttpStatusCode.NotFound),
                _ => When_client_sends_METHOD_URL_request_with_headers_and_json_content(HttpMethod.Post, "/search", new Dictionary<string, string> { { "CustomHeader", "CustomValue" } }, new { customerId = 123 }),
                _ => Then_response_should_contain_status_code_with_headers_and_json_content(HttpStatusCode.OK, new Dictionary<string, string> { { "CustomHeader", "123" } }, new { name = "John", id = "123" })
                );
        }
    }
}
