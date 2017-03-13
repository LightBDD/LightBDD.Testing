using System.Threading.Tasks;
using LightBDD.Framework;
using LightBDD.XUnit2;

[assembly: LightBddScope]

namespace LightBDD.Testing.Tests.Acceptance
{
    [FeatureDescription(
@"In order to effectively test apis,
as developer,
I want an define api tests easily")]
    public class TestableHttpClient_feature:FeatureFixture
    {
        public TestableHttpClient_feature(Xunit.Abstractions.ITestOutputHelper output) : base(output)
        {
        }

        [Scenario]
        public Task Retrieving_StatusCode_from_Get_operation()
        {
            return null;
            /*return Runner.RunScenarioAsync(
                _=>);*/
        }
    }
}
