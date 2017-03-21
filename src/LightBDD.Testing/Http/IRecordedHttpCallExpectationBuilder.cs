using System;

namespace LightBDD.Testing.Http
{
    [Flags]
    public enum ReplacementOptions
    {
        None = 0,
        Content = 1,
        Headers = 2,
        Uri = 4,
        ContentAndHeaders = Content | Headers,
        ContentAndUri = Content | Uri,
        HeadersAndUri = Headers | Uri,
        All = Headers | Uri | Content
    }

    public interface IRecordedHttpCallExpectationBuilder
    {
        IRecordedHttpCallExpectationBuilder ForRequest(Func<ITestableHttpRequest, bool> requestPredicate);
        IRecordedHttpCallExpectationBuilder ExpectResponse(Func<ITestableHttpResponse, bool> responsePredicate);
        IRecordedHttpCallExpectationBuilder WithReplacement(string regex, string replacement, ReplacementOptions options = ReplacementOptions.All);
        IRecordedHttpCallExpectationBuilder WithRequestMatch(Func<ITestableHttpRequest, ITestableHttpRequest, bool> requestMatch);
    }
}