using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios.Extended;
using LightBDD.XUnit2;
using Xunit;

[assembly: LightBddScope]

namespace LightBDD.Testing.Tests.Acceptance
{
    [FeatureDescription(
@"In order to effectively test apis,
as developer,
I want an define api tests easily")]
    public partial class TestableHttpClient_feature
    {
        [Scenario]
        public Task Performing_GET()
        {
            return Runner.RunScenarioActionsAsync(
                _ => Given_mock_http_server(),
                _ => Given_test_http_client(),
                _ => Given_server_configured_for_METHOD_URL_to_return_status_code_with_json_content(HttpMethod.Get, "/customer/123", HttpStatusCode.OK, new { name = "John", id = "123" }),
                _ => When_client_sends_METHOD_URL_request(HttpMethod.Get, "/customer/123"),
                _ => Then_response_should_contain_status_code_and_json_content(HttpStatusCode.OK, new { name = "John", id = "123" })
                );
        }

        [Scenario]
        public Task Performing_operations_with_content()
        {
            return Runner.RunScenarioActionsAsync(
                _ => Given_mock_http_server(),
                _ => Given_test_http_client(),
                _ => Given_server_configured_for_METHOD_URL_and_json_content_to_return_status_code(HttpMethod.Post, "/customers", new { name = "John" }, HttpStatusCode.Created),
                _ => When_client_sends_METHOD_URL_request_with_json_content(HttpMethod.Post, "/customers", new { name = "Josh" }),
                _ => Then_response_should_contain_status_code(HttpStatusCode.NotImplemented),
                _ => When_client_sends_METHOD_URL_request_with_json_content(HttpMethod.Post, "/customers", new { name = "John" }),
                _ => Then_response_should_contain_status_code(HttpStatusCode.Created)
                );
        }

        [Scenario]
        public void Accessing_LastResponse_without_making_request()
        {
            Runner.RunScenario(
                _ => Given_mock_http_server(),
                _ => Given_test_http_client(),
                _ => Then_LastResponse_should_be_not_null_but_also_not_valid(),
                _ => Then_attempt_to_use_LastResponse_properties_should_end_with_InvalidOperationException()
                );
        }

        [Scenario]
        public async Task Response_processing_methods_should_provide_actual_response_details_on_failure()
        {
            await Runner.RunScenarioActionsAsync(
                _ => Given_mock_http_server(),
                _ => Given_test_http_client(),
                _ => Given_server_configured_for_METHOD_URL_to_return_status_code_with_json_content(HttpMethod.Get, "/customers/123", HttpStatusCode.NotFound, new { Error = "No such user" }),
                _ => When_client_sends_METHOD_URL_request(HttpMethod.Get, "/customers/123"),
                _ => Then_attempt_to_validate_successful_response_code_should_end_with_exception_containing_actual_response_and_its_content_in_body(),
                _ => Then_attempt_to_process_expected_successful_body_should_end_with_exception_containing_actual_response_and_its_content_in_body()
                );
        }
    }
}
