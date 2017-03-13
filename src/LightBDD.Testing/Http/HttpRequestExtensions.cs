using System.Dynamic;
using System.IO;
using Newtonsoft.Json;

namespace LightBDD.Testing.Http
{
    public static class HttpRequestExtensions
    {
        public static string GetContentAsString(this HttpRequest request)
        {
            using (var reader = new StreamReader(new MemoryStream(request.Content), request.ContentEncoding))
                return reader.ReadToEnd();
        }

        public static TModel GetContentAsJson<TModel>(this HttpRequest request, JsonSerializerSettings settings = null)
        {
            return JsonConvert.DeserializeObject<TModel>(request.GetContentAsString(), settings);
        }

        public static dynamic GetContentAsJson(this HttpRequest request, JsonSerializerSettings settings = null)
        {
            return JsonConvert.DeserializeObject<ExpandoObject>(request.GetContentAsString(), settings);
        }
    }
}