using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using LightBDD.Framework;
using LightBDD.Testing.Http.Implementation;
using Newtonsoft.Json;

namespace LightBDD.Testing.Http
{
    public static class TestableHttpResponseExtensions
    {
        public static T ToJson<T>(this ITestableHttpResponse response, JsonSerializerSettings settings = null) => JsonConvert.DeserializeObject<T>(response.ToStringContent(), settings);
        public static string ToStringContent(this ITestableHttpResponse response) => response.Content.ContentEncoding.GetString(response.Content.Content);
        public static dynamic ToJson(this ITestableHttpResponse response, JsonSerializerSettings settings = null) => JsonConvert.DeserializeObject<ExpandoObject>(response.ToStringContent(), settings);

        public static IEnumerable<dynamic> ToJsonCollection(this ITestableHttpResponse response, JsonSerializerSettings settings = null) => response.ToJson<ExpandoObject[]>(settings);
        public static T ToAnonymousJson<T>(this ITestableHttpResponse response, T expectedModel, JsonSerializerSettings settings = null) => JsonConvert.DeserializeAnonymousType(response.ToStringContent(), expectedModel, settings);
        public static ITestableHttpResponse PrintResponseInComments(this ITestableHttpResponse response)
        {
            var comment = $"Response for: {response.Request.Method} {response.Request.Uri}:\n{response.DumpToString()}";
            StepExecution.Current.Comment(comment);
            return response;
        }

        public static string DumpToString(this ITestableHttpResponse response)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"StatusCode: HTTP {(int)response.StatusCode} {response.StatusCode}");
            builder.AppendLine("Headers:");
            foreach (var header in response.Headers)
                builder.AppendLine($"- {header.Key}: {header.Value}");
            builder.AppendLine("Content:");
            builder.AppendLine(response.ToStringContent());
            return builder.ToString();
        }

        public static ITestableHttpResponse EnsureSuccessStatusCode(this ITestableHttpResponse response)
        {
            if ((int)response.StatusCode < 200 || (int)response.StatusCode >= 300)
                throw new TestableHttpResponseException($"Expected response with successful status code, but got '{response.StatusCode}'", response);
            return response;
        }

        public static ITestableHttpResponse EnsureStatusCode(this ITestableHttpResponse response, HttpStatusCode code)
        {
            if (response.StatusCode != code)
                throw new TestableHttpResponseException($"Expected '{code}' response status code, but got '{response.StatusCode}'.", response);
            return response;
        }

        public static T ProcessInResponseContext<T>(this ITestableHttpResponse response, Func<ITestableHttpResponse, T> action)
        {
            try
            {
                return action(response);
            }
            catch (Exception e) when (!(e is TestableHttpResponseException))
            {
                throw new TestableHttpResponseException(e.Message, response, e);
            }
        }

        public static ITestableHttpResponse ProcessInResponseContext(this ITestableHttpResponse response, Action<ITestableHttpResponse> action)
        {
            try
            {
                action(response);
                return response;
            }
            catch (Exception e) when (!(e is TestableHttpResponseException))
            {
                throw new TestableHttpResponseException(e.Message, response, e);
            }
        }

        public static async Task<ITestableHttpResponse> ThenProcessAsync(this Task<ITestableHttpResponse> promise, Action<ITestableHttpResponse> action)
        {
            var response = await promise;
            return response.ProcessInResponseContext(action);
        }

        public static async Task<T> ThenProcessAsync<T>(this Task<ITestableHttpResponse> promise, Func<ITestableHttpResponse, T> action)
        {
            var response = await promise;
            return response.ProcessInResponseContext(action);
        }

        public static bool IsValidResponse(this ITestableHttpResponse response)
        {
            return response != null && !(response is NoTestableHttpResponse);
        }

        public static FileInfo LogResponseOnDisk(this ITestableHttpResponse response)
        {
            return TestableHttpResponseLogger.LogResponse(response);
        }
    }
}