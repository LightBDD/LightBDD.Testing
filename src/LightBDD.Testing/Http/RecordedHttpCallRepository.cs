using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using LightBDD.Testing.Http.Implementation;
using Newtonsoft.Json;

namespace LightBDD.Testing.Http
{
    public class RecordedHttpCallRepository
    {
        private readonly string _storageDirectory;
        private readonly ConcurrentStack<ITestableHttpResponse> _items = new ConcurrentStack<ITestableHttpResponse>();
        private int _counter = 0;
        private const string Ext = "json";

        public RecordedHttpCallRepository(string storageDirectory)
        {
            _storageDirectory = storageDirectory;
            if (!Directory.Exists(_storageDirectory))
                Directory.CreateDirectory(_storageDirectory);

            ReadFiles();
        }

        private void ReadFiles()
        {
            var files = Directory.GetFileSystemEntries(_storageDirectory, $"*.{Ext}", SearchOption.TopDirectoryOnly).OrderBy(p => p);
            foreach (var path in files)
                _items.Push(ReadFile(path));
        }

        private static ITestableHttpResponse ReadFile(string path)
        {
            try
            {
                return JsonConvert.DeserializeObject<RecordedHttpResponse>(File.ReadAllText(path)).Convert();
            }
            catch (Exception e)
            {
                throw new IOException($"Unable to read recorded http call from '{path}': {e.Message}", e);
            }
        }

        public IEnumerable<ITestableHttpResponse> GetAll()
        {
            return _items;
        }

        public void Add(ITestableHttpResponse response)
        {
            _items.Push(response);

            var path = Path.Combine(_storageDirectory, $"{DateTime.UtcNow:yyyyMMdd-HHmmss}-{Interlocked.Increment(ref _counter):D5}.{Ext}");

            File.WriteAllText(path, JsonConvert.SerializeObject(new RecordedHttpResponse(response), Formatting.Indented));
        }

        class RecordedHttpResponse
        {
            public RecordedHttpContent Content { get; set; }
            public Dictionary<string, string> Headers { get; set; }
            public string ReasonPhrase { get; set; }
            public HttpStatusCode StatusCode { get; set; }
            public RecordedHttpRequest Request { get; set; }

            public RecordedHttpResponse() { }
            public RecordedHttpResponse(ITestableHttpResponse model)
            {
                Content = new RecordedHttpContent(model.Content);
                Headers = model.Headers.ToDictionary(kv => kv.Key, kv => kv.Value);
                ReasonPhrase = model.ReasonPhrase;
                StatusCode = model.StatusCode;
                Request = new RecordedHttpRequest(model.Request);
            }

            public ITestableHttpResponse Convert()
            {
                return new TestableHttpResponse(Request.Convert(), StatusCode, ReasonPhrase, Content.Convert(), Headers);
            }
        }
        class RecordedHttpContent
        {
            public RecordedHttpContent() { }
            public RecordedHttpContent(ITestableHttpContent model)
            {
                ContentEncoding = model.ContentEncoding.WebName;
                Content = model.ContentEncoding.GetString(model.Content);
                ContentType = model.ContentType;
                ContentLength = model.ContentLength;
                Headers = model.Headers.ToDictionary(kv => kv.Key, kv => kv.Value);
            }

            public string ContentEncoding { get; set; }
            public string Content { get; set; }
            public string ContentType { get; set; }
            public long ContentLength { get; set; }
            public Dictionary<string, string> Headers { get; set; }

            public ITestableHttpContent Convert()
            {
                var encoding = Encoding.GetEncoding(ContentEncoding);
                return new MockHttpContent(encoding.GetBytes(Content), encoding, ContentType, Headers);
            }
        }
        class RecordedHttpRequest
        {
            public RecordedHttpRequest() { }
            public RecordedHttpRequest(ITestableHttpRequest model)
            {
                RelativeUri = model.RelativeUri;
                BaseUri = model.BaseUri.ToString();
                Headers = model.Headers.ToDictionary(kv => kv.Key, kv => kv.Value);
                Method = model.Method.ToString();
                Content = new RecordedHttpContent(model.Content);
            }

            public string RelativeUri { get; set; }
            public string BaseUri { get; set; }
            public Dictionary<string, string> Headers { get; set; }
            public string Method { get; set; }
            public RecordedHttpContent Content { get; set; }

            public ITestableHttpRequest Convert()
            {
                return new MockHttpRequest(new Uri(BaseUri), new HttpMethod(Method), RelativeUri, Content.Convert(), Headers);
            }
        }
    }
}