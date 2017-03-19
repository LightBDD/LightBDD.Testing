using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace LightBDD.Testing.Http
{
    public class RecordingHttpProxy : IDisposable
    {
        public HttpClient Proxy { get; }
        public Uri BaseAddress => _server.BaseAddress;

        private readonly bool _offline;
        private readonly MockHttpServer _server;

        public RecordingHttpProxy(int proxyPort, Uri targetUrl, bool offline)
        {
            Proxy = new HttpClient { BaseAddress = targetUrl };
            _offline = offline;
            _server = MockHttpServer.Start(proxyPort, cfg => ConfigureServer(cfg, offline));
        }

        private MockHttpServerConfigurator ConfigureServer(MockHttpServerConfigurator cfg, bool offline)
        {
            if (offline)
                return cfg.ForRequest(r => true)
                    .RespondStringContent(
                        HttpStatusCode.NotImplemented,
                        "Proxy works in offline mode and no mapping is specified for this request.",
                        Encoding.UTF8,
                        "text")
                    .Apply();

            return cfg.ForRequest(r => true).Respond(ProxyAndRecordAsync).Apply();
        }

        private async Task ProxyAndRecordAsync(MockHttpRequest req, MockHttpResponse rsp)
        {
            var request = new HttpRequestMessage(req.Method, req.RelativeUri) { Content = req.ContentLength >= 0 ? new ByteArrayContent(req.Content) : null };
            CopyHeaders(req, request);

            var response = await Proxy.SendAsync(request);
            rsp.SetStatusCode(response.StatusCode)
                .SetContent(
                    await response.Content.ReadAsByteArrayAsync(),
                    response.Content.Headers.ContentEncoding.Select(Encoding.GetEncoding).FirstOrDefault() ??
                    Encoding.UTF8,
                    response.Content.Headers.ContentType.MediaType)
                .SetHeaders(CollectResponseHeaders(response));
        }

        private static IEnumerable<KeyValuePair<string, string>> CollectResponseHeaders(HttpResponseMessage response)
        {
            return response.Headers.Select(kv => new KeyValuePair<string, string>(kv.Key, kv.Value.FirstOrDefault()));
        }

        private static void CopyHeaders(MockHttpRequest from, HttpRequestMessage to)
        {
            foreach (var headerKey in from.Headers.AllKeys)
            {
                var headerValue = from.Headers[headerKey];
                if (!to.Headers.TryAddWithoutValidation(headerKey, headerValue))
                    to.Content.Headers.Add(headerKey, headerValue);
            }
        }

        public void Dispose()
        {
            _server.Dispose();
        }
    }
}
