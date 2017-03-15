using System.Net.Http;
using System.Threading.Tasks;
using LightBDD.Testing.Http.Implementation;

namespace LightBDD.Testing.Http
{
    public static class TestableHttpResponseMessages
    {
        public static async Task<ITestableHttpResponseMessage> FromResponseAsync(HttpResponseMessage response)
        {
            return new TestableHttpResponseMessage(response, await response.Content.ReadAsStringAsync());
        }
    }
}