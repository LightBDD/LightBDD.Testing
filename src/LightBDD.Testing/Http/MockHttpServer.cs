using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using LightBDD.Testing.Http.Implementation;

namespace LightBDD.Testing.Http
{
    public class MockHttpServer : IDisposable
    {
        private static readonly HttpRequestProcessor NotImplementedProcessor = new HttpRequestProcessor(r => true, (req, resp) =>
        {
            resp.SetStatusCode(HttpStatusCode.NotImplemented);
            return Task.CompletedTask;
        });

        public Uri BaseAddress { get; }
        private readonly HttpListener _listener;
        private readonly Task _listenerTask;
        private readonly object _sync = new object();
        private HttpRequestProcessor[] _processors = new HttpRequestProcessor[0];
        private readonly IDictionary<Guid, Task> _pendingTasks = new ConcurrentDictionary<Guid, Task>();

        public static MockHttpServer Start(int port) => Start(port, cfg => cfg);
        public static MockHttpServer Start(int port, Func<MockHttpServerConfigurator, MockHttpServerConfigurator> configurator)
        {
            return new MockHttpServer(port, configurator);
        }

        private MockHttpServer(int port, Func<MockHttpServerConfigurator, MockHttpServerConfigurator> configurator)
        {
            BaseAddress = new Uri($"http://localhost:{port}");
            Reconfigure(true, configurator);

            _listener = new HttpListener();
            _listener.Prefixes.Add(BaseAddress.ToString());
            _listener.Start();
            _listenerTask = Task.Run(Listen);
        }

        private async Task Listen()
        {
            while (_listener.IsListening)
            {
                try
                {
                    var ctx = await _listener.GetContextAsync();
                    ScheduleProcessRequest(ctx);
                }
                catch (Exception) { }
            }
            await Task.WhenAll(_pendingTasks.Values);
        }

        private void ScheduleProcessRequest(HttpListenerContext ctx)
        {
            var id = Guid.NewGuid();
            var task = new Task(async () =>
            {
                await ProcessRequest(ctx);
                _pendingTasks.Remove(id);
            });
            _pendingTasks.Add(id, task);
            task.Start();
        }

        public void Reconfigure(bool replaceExisting, Func<MockHttpServerConfigurator, MockHttpServerConfigurator> configurator)
        {
            var cfg = configurator(new MockHttpServerConfigurator());
            lock (_sync)
            {
                var newProcessors = (replaceExisting ? cfg.Processors : (cfg.Processors.Concat(_processors))).ToArray();
                _processors = newProcessors;
            }
        }

        private async Task ProcessRequest(HttpListenerContext ctx)
        {
            var response = new HttpResponse(ctx.Response);
            try
            {
                var content = await ReadContentAsync(ctx);
                var request = new HttpRequest(ctx.Request, content, BaseAddress);

                var processor = _processors.FirstOrDefault(p => p.Match(request)) ?? NotImplementedProcessor;
                await processor.ProcessRequestAsync(request, response);
                await response.SendResponseAsync();
            }
            catch (Exception)
            {
                ctx.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }
            finally
            {
                response.Close();
            }
        }

        private static async Task<byte[]> ReadContentAsync(HttpListenerContext ctx)
        {
            if (ctx.Request.ContentLength64 < 0)
                return new byte[0];
            var buffer = new byte[ctx.Request.ContentLength64];
            await ctx.Request.InputStream.ReadAsync(buffer, 0, (int)ctx.Request.ContentLength64);
            return buffer;
        }

        public void Dispose()
        {
            if (!_listener.IsListening)
                return;
            _listener.Stop();
            _listenerTask.Wait();
        }
    }
}
