using System;
using System.Collections.Generic;

namespace LightBDD.Testing.Http.Implementation
{
    internal class RecordedHttpCallExpectationBuilder : IRecordedHttpCallExpectationBuilder
    {
        private Func<ITestableHttpRequest, bool> _requestPredicate;
        private Func<ITestableHttpResponse, bool> _responsePredicate;
        private readonly List<Tuple<string,string,ReplacementOptions>> _replacements=new List<Tuple<string, string, ReplacementOptions>>();

        public RecordedHttpCallExpectation Build()
        {
            return new RecordedHttpCallExpectation(_requestPredicate,_responsePredicate,_replacements.ToArray());
        }

        public IRecordedHttpCallExpectationBuilder ForRequest(Func<ITestableHttpRequest, bool> requestPredicate)
        {
            _requestPredicate = requestPredicate;
            return this;
        }

        public IRecordedHttpCallExpectationBuilder ExpectResponse(Func<ITestableHttpResponse, bool> responsePredicate)
        {
            _responsePredicate = responsePredicate;
            return this;
        }

        public IRecordedHttpCallExpectationBuilder WithReplacement(string regex, string replacement,ReplacementOptions options = ReplacementOptions.All)
        {
            _replacements.Add(Tuple.Create(regex, replacement, options));
            return this;
        }
    }
}