using System;
using System.Net.Http;

namespace LightBDD.Testing.Http
{
    public class TestableHttpClient : IDisposable
    {
        private readonly HttpClient _client = new HttpClient();

        public TestableHttpClient(string baseUrl)
        {
            if (baseUrl == null)
                throw new ArgumentNullException(nameof(baseUrl));

            _client.BaseAddress = new Uri(baseUrl);
        }

        public void Dispose()
        {
            _client.Dispose();
        }
    }
}
