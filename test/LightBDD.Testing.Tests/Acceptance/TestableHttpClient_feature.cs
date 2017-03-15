using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios.Extended;
using LightBDD.XUnit2;

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
    }
}
