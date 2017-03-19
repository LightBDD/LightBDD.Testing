using System;
using System.IO;
using System.Threading;

namespace LightBDD.Testing.Http
{
    public class TestableHttpResponseLogger
    {
        public static readonly string DefaultHttpResponseLogPath = DetermineResponseLogPath();

        private static string DetermineResponseLogPath()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "HttpResponseLogs");
            try
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }
            catch
            {
                path = AppContext.BaseDirectory;
            }
            return path;
        }

        private static int _counter;

        public static FileInfo LogResponse(ITestableHttpResponse response)
        {
            var path = Path.Combine(DefaultHttpResponseLogPath, $"{DateTime.UtcNow.ToString("yyyyMMdd-HHmmss") + Interlocked.Increment(ref _counter)}.txt");
            File.WriteAllText(path, response.DumpToString());
            return new FileInfo(path);
        }
    }
}