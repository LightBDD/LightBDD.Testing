using System;
using System.IO;
using System.Text;

namespace LightBDD.Testing.Http
{
    public class TestableHttpResponseException : InvalidOperationException
    {
        public ITestableHttpResponse ActualResponse { get; }
        public FileInfo ResponseLogFile { get; }

        public TestableHttpResponseException(string message, ITestableHttpResponse actualResponse, Exception inner = null)
            : this(FormatMessage(message, actualResponse), actualResponse, inner) { }

        private TestableHttpResponseException(Tuple<string, FileInfo> formattedMessage, ITestableHttpResponse actualResponse, Exception inner)
            : base(formattedMessage.Item1, inner)
        {
            ActualResponse = actualResponse;
            ResponseLogFile = formattedMessage.Item2;
        }

        private static Tuple<string, FileInfo> FormatMessage(string message, ITestableHttpResponse actualResponse)
        {
            if (!actualResponse.IsValidResponse())
                return Tuple.Create($"{message}\n\nActual response object is invalid (no response received so far)", (FileInfo)null);

            var request = actualResponse.OriginalResponse.RequestMessage;
            var logFile = actualResponse.LogResponseOnDisk();
            return Tuple.Create($"{message}\n\nActual response for {request.Method} {request.RequestUri}:\n{CreateTrimmedDump(actualResponse, 2048)}\nFull response log: {logFile.FullName}", logFile);
        }

        private static string CreateTrimmedDump(ITestableHttpResponse actualResponse, int maxLength)
        {
            var dump = actualResponse.DumpToString();
            if (dump.Length > maxLength)
                dump = dump.Substring(0, maxLength) + "...";
            return dump;
        }
    }
}