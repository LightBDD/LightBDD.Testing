using System;
using System.Text;

namespace LightBDD.Testing.Http
{
    public class TestableHttpResponseException : InvalidOperationException
    {
        public ITestableHttpResponse ActualResponse { get; }

        public TestableHttpResponseException(string message, ITestableHttpResponse actualResponse, Exception inner = null)
            : base(FormatMessage(message, actualResponse), inner)
        {
            ActualResponse = actualResponse;
        }

        private static string FormatMessage(string message, ITestableHttpResponse actualResponse)
        {
            if (!actualResponse.IsValidResponse())
                return $"{message}\n\nActual response object is invalid (no response received so far)";

            var request = actualResponse.OriginalResponse.RequestMessage;
            return $"{message}\n\nActual response for {request.Method} {request.RequestUri}: {actualResponse.StatusCode}\nContent({actualResponse.Content.Length}): {actualResponse.Content}";
        }
    }
}