using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LightBDD.Testing.Http.Implementation
{
    internal class RecordedHttpCallExpectation
    {
        private readonly Func<ITestableHttpRequest, bool> _requestPredicate;
        private readonly Func<ITestableHttpResponse, bool> _responsePredicate;
        private readonly Func<ITestableHttpRequest, ITestableHttpRequest, bool> _requestMatch;
        private readonly Tuple<Regex, string, ReplacementOptions>[] _replacements;

        public RecordedHttpCallExpectation(Func<ITestableHttpRequest, bool> requestPredicate, Func<ITestableHttpResponse, bool> responsePredicate, Func<ITestableHttpRequest, ITestableHttpRequest, bool> requestMatch, Tuple<Regex, string, ReplacementOptions>[] replacements)
        {
            _requestPredicate = requestPredicate;
            _responsePredicate = responsePredicate;
            _requestMatch = requestMatch;
            _replacements = replacements;
        }

        public bool Match(ITestableHttpRequest request) => _requestPredicate.Invoke(ApplyReplacements(request));

        public ITestableHttpResponse PrepareForReplay(ITestableHttpRequest request, RecordedHttpCallRepository repository)
        {
            foreach (var recordedResponse in repository.GetAll())
            {
                if (!_requestMatch(request, ApplyReplacements(recordedResponse.Request)))
                    continue;

                var updatedResponse = ApplyReplacements(recordedResponse);
                if (!_responsePredicate.Invoke(updatedResponse))
                    continue;

                return updatedResponse;
            }

            throw new InvalidOperationException("No suitable recorded message has been found");
        }

        private ITestableHttpRequest ApplyReplacements(ITestableHttpRequest request)
        {
            var content = request.GetContentAsString();
            var relativeUri = request.RelativeUri;
            var headers = request.Headers.ToDictionary(kv => kv.Key, kv => kv.Value);
            foreach (var replacement in _replacements)
            {
                ReplaceIf(replacement, ref content, ReplacementOptions.Content);
                ReplaceIf(replacement, ref relativeUri, ReplacementOptions.Uri);
                ReplaceHeaders(replacement, headers);
            }

            var httpContent = new MockHttpContent(request.Content.ContentEncoding.GetBytes(content), request.Content.ContentEncoding, request.Content.ContentType, request.Content.Headers);
            return new MockHttpRequest(request.BaseUri, request.Method, relativeUri, httpContent, headers);
        }

        private ITestableHttpResponse ApplyReplacements(ITestableHttpResponse response)
        {
            var content = response.ToStringContent();
            var headers = response.Headers.ToDictionary(kv => kv.Key, kv => kv.Value);
            foreach (var replacement in _replacements)
            {
                ReplaceIf(replacement, ref content, ReplacementOptions.Content);
                ReplaceHeaders(replacement, headers);
            }

            var httpContent = new MockHttpContent(response.Content.ContentEncoding.GetBytes(content), response.Content.ContentEncoding, response.Content.ContentType, response.Content.Headers);
            return new TestableHttpResponse(response.Request, response.StatusCode, response.ReasonPhrase, httpContent, headers);
        }

        private void ReplaceHeaders(Tuple<Regex, string, ReplacementOptions> replacement, Dictionary<string, string> headers)
        {
            if ((replacement.Item3 & ReplacementOptions.Headers) == 0)
                return;
            foreach (var header in headers)
                headers[header.Key] = replacement.Item1.Replace(header.Value, replacement.Item2);
        }

        private static void ReplaceIf(Tuple<Regex, string, ReplacementOptions> replacement, ref string content, ReplacementOptions expectedFlag)
        {
            if ((replacement.Item3 & expectedFlag) > 0)
                content = replacement.Item1.Replace(content, replacement.Item2);
        }
    }
}