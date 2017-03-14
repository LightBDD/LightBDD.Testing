using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios.Extended;
using LightBDD.XUnit2;
using Xunit;

namespace LightBDD.Testing.Tests.Acceptance
{
    [FeatureDescription(
@"In order to mock Apis effectively,
as developer,
I want to instantiate in-process mock http server
and configure it at runtime to mock requests")]
    public partial class MockHttpServer_feature
    {
        [Scenario]
        [InlineData("GET")]
        [InlineData("PUT")]
        [InlineData("POST")]
        [InlineData("DELETE")]
        [InlineData("PATCH")]
        public async Task Server_should_return_NotImplemented_for_unconfigured_paths(string method)
        {
            await Runner.RunScenarioActionsAsync(
                _ => Given_mock_http_server(),
                _ => When_client_performs_METHOD_URL_request(new HttpMethod(method), "/not_configured_path"),
                _ => Then_the_response_should_have_status_code(HttpStatusCode.NotImplemented));
        }

        [Scenario]
        public async Task Server_should_return_preconfigured_status_code()
        {
            await Runner.RunScenarioActionsAsync(
                _ => Given_mock_http_server(),
                _ => Given_server_configured_for_METHOD_URL_to_return_status_code(HttpMethod.Get, "/status", HttpStatusCode.OK),
                _ => Given_server_configured_for_METHOD_URL_to_return_status_code(HttpMethod.Post, "/status", HttpStatusCode.Accepted),

                _ => When_client_performs_METHOD_URL_request(HttpMethod.Get, "/status"),
                _ => Then_the_response_should_have_status_code(HttpStatusCode.OK),
                _ => When_client_performs_METHOD_URL_request(HttpMethod.Post, "/status"),
                _ => Then_the_response_should_have_status_code(HttpStatusCode.Accepted),

                _ => When_client_performs_METHOD_URL_request(HttpMethod.Put, "/status"),
                _ => Then_the_response_should_have_status_code(HttpStatusCode.NotImplemented)
                );
        }

        [Scenario]
        public async Task Server_should_allow_reconfiguration_at_runtime()
        {
            await Runner.RunScenarioActionsAsync(
                _ => Given_mock_http_server(),
                _ => Given_server_configured_for_METHOD_URL_to_return_status_code(HttpMethod.Get, "/status", HttpStatusCode.OK),
                _ => Given_server_configured_for_METHOD_URL_to_return_status_code(HttpMethod.Get, "/customers", HttpStatusCode.OK),

                _ => When_client_performs_METHOD_URL_request(HttpMethod.Get, "/status"),
                _ => Then_the_response_should_have_status_code(HttpStatusCode.OK),
                _ => When_client_performs_METHOD_URL_request(HttpMethod.Get, "/customers"),
                _ => Then_the_response_should_have_status_code(HttpStatusCode.OK),

                _ => When_server_reconfigures_METHOD_URL_to_return_status_code_and_DISCARD_others(HttpMethod.Get, "/status", HttpStatusCode.ServiceUnavailable, false),
                _ => When_client_performs_METHOD_URL_request(HttpMethod.Get, "/status"),
                _ => Then_the_response_should_have_status_code(HttpStatusCode.ServiceUnavailable),
                _ => When_client_performs_METHOD_URL_request(HttpMethod.Get, "/customers"),
                _ => Then_the_response_should_have_status_code(HttpStatusCode.OK),

                _ => When_server_reconfigures_METHOD_URL_to_return_status_code_and_DISCARD_others(HttpMethod.Get, "/customers", HttpStatusCode.BadRequest, true),
                _ => When_client_performs_METHOD_URL_request(HttpMethod.Get, "/customers"),
                _ => Then_the_response_should_have_status_code(HttpStatusCode.BadRequest),
                _ => When_client_performs_METHOD_URL_request(HttpMethod.Get, "/status"),
                _ => Then_the_response_should_have_status_code(HttpStatusCode.NotImplemented)
                );
        }

        [Scenario]
        public async Task Server_should_be_thread_safe()
        {
            await Runner.RunScenarioActionsAsync(
                _ => Given_mock_http_server(),
                _ => Then_server_should_allow_reconfiguration_under_load());
        }

        [Scenario]
        public async Task Server_should_allow_configuration_based_on_request_body()
        {
            await Runner.RunScenarioActionsAsync(
                _ => Given_mock_http_server(),
                _ => Given_server_configured_for_METHOD_URL_to_return_status_code(HttpMethod.Post, "/password", HttpStatusCode.BadRequest),
                _ => Given_server_configured_for_METHOD_URL_and_body_to_return_status_code(HttpMethod.Post, "/password", "magic_text", HttpStatusCode.Accepted),
                _ => When_client_performs_METHOD_URL_request_with_content(HttpMethod.Post, "/password", "some_text"),
                _ => Then_the_response_should_have_status_code(HttpStatusCode.BadRequest),
                _ => When_client_performs_METHOD_URL_request_with_content(HttpMethod.Post, "/password", "magic_text"),
                _ => Then_the_response_should_have_status_code(HttpStatusCode.Accepted)
                );
        }

        class JsonModel
        {
            public string Name { get; set; }
        }

        [Scenario]
        public async Task Server_should_allow_configuration_based_on_request_json_body()
        {
            await Runner.RunScenarioActionsAsync(
                _ => Given_mock_http_server(),
                _ => Given_server_configured_for_METHOD_URL_to_return_status_code(HttpMethod.Post, "/customers", HttpStatusCode.BadRequest),
                _ => Given_server_configured_for_METHOD_URL_and_json_body_to_return_status_code(HttpMethod.Post, "/customers", (JsonModel body) => body.Name == "John", HttpStatusCode.Accepted),

                _ => When_client_performs_METHOD_URL_request_with_json_content(HttpMethod.Post, "/customers", new { Name = "Josh" }),
                _ => Then_the_response_should_have_status_code(HttpStatusCode.BadRequest),

                _ => When_client_performs_METHOD_URL_request_with_json_content(HttpMethod.Post, "/customers", new { Name = "John" }),
                _ => Then_the_response_should_have_status_code(HttpStatusCode.Accepted),

                _ => When_client_performs_METHOD_URL_request_with_json_content(HttpMethod.Post, "/customers", new JsonModel { Name = "John" }),
                _ => Then_the_response_should_have_status_code(HttpStatusCode.Accepted)
                );
        }

        [Scenario]
        public async Task Server_should_allow_configuring_responses_with_content()
        {
            await Runner.RunScenarioActionsAsync(
                _ => Given_mock_http_server(),

                _ => Given_server_configured_for_METHOD_URL_to_return_status_code_with_content(HttpMethod.Get, "/status", HttpStatusCode.OK, "all good"),
                _ => When_client_performs_METHOD_URL_request(HttpMethod.Get, "/status"),
                _ => Then_the_response_should_have_status_code(HttpStatusCode.OK),
                _ => Then_the_response_should_have_content("all good"),

                _ => Given_server_configured_for_METHOD_URL_to_return_status_code_with_json_content(HttpMethod.Get, "/customer/123", HttpStatusCode.OK, new { name = "John", id = "123" }),
                _ => When_client_performs_METHOD_URL_request(HttpMethod.Get, "/customer/123"),
                _ => Then_the_response_should_have_status_code(HttpStatusCode.OK),
                _ => Then_the_response_should_have_content("{\"name\":\"John\",\"id\":\"123\"}")
            );
        }

        [Scenario]
        public async Task Server_should_allow_mappings_that_would_expire_after_specified_amount_of_calls()
        {
            await Runner.RunScenarioActionsAsync(
                _ => Given_mock_http_server(),

                _ => Given_server_configured_for_METHOD_URL_to_return_status_code(HttpMethod.Get, "/status", HttpStatusCode.OK),
                _ => Given_server_configured_for_METHOD_URL_to_return_status_code_for_next_NUMBER_of_calls(HttpMethod.Get, "/status", HttpStatusCode.BadRequest, 2),

                _ => When_client_performs_METHOD_URL_request(HttpMethod.Get, "/status"),
                _ => Then_the_response_should_have_status_code(HttpStatusCode.BadRequest),

                _ => When_client_performs_METHOD_URL_request(HttpMethod.Get, "/status"),
                _ => Then_the_response_should_have_status_code(HttpStatusCode.BadRequest),

                _ => When_client_performs_METHOD_URL_request(HttpMethod.Get, "/status"),
                _ => Then_the_response_should_have_status_code(HttpStatusCode.OK)
            );
        }

        [Scenario]
        public async Task Server_should_allow_mappings_that_would_expire_after_specified_time()
        {
            await Runner.RunScenarioActionsAsync(
                _ => Given_mock_http_server(),

                _ => Given_server_configured_for_METHOD_URL_to_return_status_code(HttpMethod.Get, "/status", HttpStatusCode.OK),
                _ => Given_server_configured_for_METHOD_URL_to_return_status_code_for_next_NUMBER_seconds(HttpMethod.Get, "/status", HttpStatusCode.BadRequest, 2),

                _ => When_client_performs_METHOD_URL_request(HttpMethod.Get, "/status"),
                _ => Then_the_response_should_have_status_code(HttpStatusCode.BadRequest),

                _ => When_client_performs_METHOD_URL_request(HttpMethod.Get, "/status"),
                _ => Then_the_response_should_have_status_code(HttpStatusCode.BadRequest),

                _ => When_time_elapses(TimeSpan.FromSeconds(2)),
                _ => When_client_performs_METHOD_URL_request(HttpMethod.Get, "/status"),
                _ => Then_the_response_should_have_status_code(HttpStatusCode.OK)
            );
        }
    }
}
