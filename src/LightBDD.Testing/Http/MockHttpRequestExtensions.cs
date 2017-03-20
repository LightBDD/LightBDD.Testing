using System.Dynamic;
using System.IO;
using Newtonsoft.Json;

namespace LightBDD.Testing.Http
{
    public static class MockHttpRequestExtensions
    {
        public static string GetContentAsString(this ITestableHttpRequest request)
        {
            using (var reader = new StreamReader(new MemoryStream(request.Content.Content), request.Content.ContentEncoding))
                return reader.ReadToEnd();
        }

        public static TModel GetContentAsJson<TModel>(this ITestableHttpRequest request, JsonSerializerSettings settings = null)
        {
            return JsonConvert.DeserializeObject<TModel>(request.GetContentAsString(), settings);
        }

        public static TModel GetContentAsAnonymousJson<TModel>(this ITestableHttpRequest request, TModel exampleModel, JsonSerializerSettings settings = null)
        {
            return JsonConvert.DeserializeAnonymousType<TModel>(request.GetContentAsString(), exampleModel, settings);
        }

        public static dynamic GetContentAsJson(this ITestableHttpRequest request, JsonSerializerSettings settings = null)
        {
            return JsonConvert.DeserializeObject<ExpandoObject>(request.GetContentAsString(), settings);
        }
    }
}