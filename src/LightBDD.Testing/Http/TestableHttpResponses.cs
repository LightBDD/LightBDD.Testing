using System.Net.Http;
using System.Threading.Tasks;
using LightBDD.Testing.Http.Implementation;

namespace LightBDD.Testing.Http
{
    public static class TestableHttpResponses
    {
        public static async Task<ITestableHttpResponse> FromResponseAsync(HttpResponseMessage response)
        {
            return new TestableHttpResponse(response, await response.Content.ReadAsStringAsync());
        }
    }
}