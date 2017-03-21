using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LightBDD.Testing.Http.Implementation;

namespace LightBDD.Testing.Http
{
    public class RecordingHttpProxy : IDisposable
    {
        private readonly Mode _mode;

        public enum Mode
        {
            Record,
            Replay
        }

        public HttpClient Proxy { get; }
        public Uri BaseAddress => _server.BaseAddress;

        private readonly MockHttpServer _server;
        private readonly ConcurrentStack<RecordedHttpCallExpectation> _expectations = new ConcurrentStack<RecordedHttpCallExpectation>();
        private readonly RecordedHttpCallRepository _repository;

        public RecordingHttpProxy(int proxyPort, Uri targetUrl, Mode mode, RecordedHttpCallRepository repository)
        {
            _mode = mode;
            _repository = repository;
            Proxy = new HttpClient { BaseAddress = targetUrl };
            _server = MockHttpServer.Start(proxyPort, cfg => ConfigureServer(cfg, mode));
        }

        private MockHttpServerConfigurator ConfigureServer(MockHttpServerConfigurator cfg, Mode mode)
        {
            if (mode != Mode.Replay)
                return cfg.ForRequest(r => true).RespondAsync(ProxyAndRecordAsync).Apply();

            DefineExpectation(b => b
                .ForRequest(r => true)
                .ExpectResponse(r => true)
                .WithRequestMatch((req, rec) =>
                    string.Equals(req.RelativeUri, rec.RelativeUri, StringComparison.OrdinalIgnoreCase)
                    && req.Method == rec.Method
                    && req.Headers.All(h => rec.Headers.ContainsKey(h.Key) && h.Value == rec.Headers[h.Key])
                    && req.GetContentAsString() == rec.GetContentAsString()));

            return cfg.ForRequest(r => true)
                .Respond(ReplayResponse)
                .Apply();
        }

        private void ReplayResponse(ITestableHttpRequest request, IMockHttpResponse response)
        {
            var expectation = _expectations.FirstOrDefault(e => e.Match(request));
            if (expectation != null)
                expectation.Replay(request, response, _repository);
            else
                ReplayNoMapping(response);
        }

        private static void ReplayNoMapping(IMockHttpResponse response)
        {
            response
                .SetStatusCode(HttpStatusCode.NotImplemented)
                .SetStringContent(
                    "Proxy works in replay mode and no mapping is specified for this request.",
                    Encoding.UTF8,
                    "text");
        }

        private async Task ProxyAndRecordAsync(ITestableHttpRequest req, IMockHttpResponse rsp)
        {
            var request = new HttpRequestMessage(req.Method, req.RelativeUri) { Content = req.Content.ContentLength >= 0 ? new ByteArrayContent(req.Content.Content) : null };
            CopyHeaders(req, request);

            var response = await Proxy.SendAsync(request);

            var content = await response.Content.ReadAsByteArrayAsync();

            _repository.Add(new TestableHttpResponse(response, content, req));

            rsp.SetStatusCode(response.StatusCode)
                .SetContent(
                    content,
                    response.Content.Headers.ContentEncoding.Select(Encoding.GetEncoding).FirstOrDefault() ??
                    Encoding.UTF8,
                    response.Content.Headers.ContentType.MediaType)
                .SetHeaders(CollectResponseHeaders(response));
        }

        private static IEnumerable<KeyValuePair<string, string>> CollectResponseHeaders(HttpResponseMessage response)
        {
            return response.Headers.Select(kv => new KeyValuePair<string, string>(kv.Key, kv.Value.FirstOrDefault()));
        }

        private static void CopyHeaders(ITestableHttpRequest from, HttpRequestMessage to)
        {
            foreach (var header in from.Headers)
            {
                if (!to.Headers.TryAddWithoutValidation(header.Key, header.Value))
                    to.Content.Headers.Add(header.Key, header.Value);
            }
        }

        public void Dispose()
        {
            _server.Dispose();
        }

        public RecordingHttpProxy DefineExpectation(Action<IRecordedHttpCallExpectationBuilder> builderFunction)
        {
            var expectationBuilder = new RecordedHttpCallExpectationBuilder();
            builderFunction.Invoke(expectationBuilder);
            _expectations.Push(expectationBuilder.Build());
            return this;
        }
    }
}
