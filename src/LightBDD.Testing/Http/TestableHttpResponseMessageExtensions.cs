using System;
using System.Dynamic;
using System.Net;
using LightBDD.Framework;
using LightBDD.Framework.Commenting;
using LightBDD.Testing.Http.Implementation;
using Newtonsoft.Json;

namespace LightBDD.Testing.Http
{
    public static class TestableHttpResponseMessageExtensions
    {
        public static T ToJson<T>(this ITestableHttpResponseMessage response, JsonSerializerSettings settings = null) => JsonConvert.DeserializeObject<T>(response.Content, settings);
        public static dynamic ToJson(this ITestableHttpResponseMessage response, JsonSerializerSettings settings = null) => JsonConvert.DeserializeObject<ExpandoObject>(response.Content, settings);
        public static T ToAnonymousJson<T>(this ITestableHttpResponseMessage response, T expectedModel, JsonSerializerSettings settings = null) => JsonConvert.DeserializeAnonymousType(response.Content, expectedModel, settings);
        public static ITestableHttpResponseMessage PrintResponseInComments(this ITestableHttpResponseMessage response)
        {
            var comment = $"Response for: {response.OriginalResponse.RequestMessage.Method} {response.OriginalResponse.RequestMessage.RequestUri}: {response.StatusCode}\n{response.Content}";
            StepExecution.Current.Comment(comment);
            return response;
        }

        public static ITestableHttpResponseMessage EnsureSuccessStatusCode(this ITestableHttpResponseMessage response)
        {
            response.OriginalResponse.EnsureSuccessStatusCode();
            return response;
        }

        public static ITestableHttpResponseMessage EnsureStatusCode(this ITestableHttpResponseMessage response, HttpStatusCode code)
        {
            if (response.StatusCode != code)
                throw new InvalidOperationException($"Expected '{code}' response status code, but got '{response.StatusCode}'.");
            return response;
        }

        public static bool IsValidResponse(this ITestableHttpResponseMessage response)
        {
            return response != null && !(response is NoTestableHttpResponseMessage);
        }
    }
}