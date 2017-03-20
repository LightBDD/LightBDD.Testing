using System;

namespace LightBDD.Testing.Http
{
    [Flags]
    public enum ReplacementOptions
    {
        None = 0,
        Content = 1,
        Headers = 2,
        Url = 4,
        ContentAndHeaders = Content | Headers,
        ContentAndUrl = Content | Url,
        HeadersAndUrl = Headers | Url,
        All = Headers | Url | Content
    }

    public interface IRecordedHttpCallExpectationBuilder
    {
        IRecordedHttpCallExpectationBuilder ForRequest(Func<ITestableHttpRequest, bool> requestPredicate);
        IRecordedHttpCallExpectationBuilder ExpectResponse(Func<ITestableHttpResponse, bool> responsePredicate);
        IRecordedHttpCallExpectationBuilder WithReplacement(string regex, string replacement, ReplacementOptions options = ReplacementOptions.All);
    }
}