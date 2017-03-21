using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LightBDD.Testing.Http.Implementation
{
    internal class RecordedHttpCallExpectationBuilder : IRecordedHttpCallExpectationBuilder
    {
        private Func<ITestableHttpRequest, bool> _requestPredicate;
        private Func<ITestableHttpResponse, bool> _responsePredicate;
        private readonly List<Tuple<Regex, string, ReplacementOptions>> _replacements = new List<Tuple<Regex, string, ReplacementOptions>>();
        private Func<ITestableHttpRequest, ITestableHttpRequest, bool> _requestMatch;

        public RecordedHttpCallExpectation Build()
        {
            Func<ITestableHttpRequest, ITestableHttpRequest, bool> defaultRequestMatch = (rec, rsp) => _requestPredicate(rsp);

            return new RecordedHttpCallExpectation(_requestPredicate, _responsePredicate, _requestMatch ?? defaultRequestMatch, _replacements.ToArray());
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

        public IRecordedHttpCallExpectationBuilder WithReplacement(string regex, string replacement, ReplacementOptions options = ReplacementOptions.All)
        {
            _replacements.Add(Tuple.Create(new Regex(regex, RegexOptions.Compiled), replacement, options));
            return this;
        }

        public IRecordedHttpCallExpectationBuilder WithRequestMatch(Func<ITestableHttpRequest, ITestableHttpRequest, bool> requestMatch)
        {
            _requestMatch = requestMatch;
            return this;
        }
    }
}