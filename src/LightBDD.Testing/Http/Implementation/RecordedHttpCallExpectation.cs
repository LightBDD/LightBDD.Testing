using System;

namespace LightBDD.Testing.Http.Implementation
{
    internal class RecordedHttpCallExpectation
    {
        private readonly Func<ITestableHttpRequest, bool> _requestPredicate;
        private readonly Func<ITestableHttpResponse, bool> _responsePredicate;
        private readonly Tuple<string, string, ReplacementOptions>[] _replacements;

        public RecordedHttpCallExpectation(Func<ITestableHttpRequest, bool> requestPredicate, Func<ITestableHttpResponse, bool> responsePredicate, Tuple<string, string, ReplacementOptions>[] replacements)
        {
            _requestPredicate = requestPredicate;
            _responsePredicate = responsePredicate;
            _replacements = replacements;
        }

        public bool Match(ITestableHttpRequest request) { throw new NotImplementedException();}
        public void Replay(ITestableHttpRequest request, IMockHttpResponse response) { throw new NotImplementedException();}
    }
}