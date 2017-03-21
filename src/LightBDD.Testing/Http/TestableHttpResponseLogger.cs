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
            var path = Path.Combine(AppContext.BaseDirectory, "ResponseLogs");
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
            var path = Path.Combine(DefaultHttpResponseLogPath, $"response-{DateTime.UtcNow:yyyyMMdd-HHmmss}-{Interlocked.Increment(ref _counter):D5}.txt");
            File.WriteAllText(path, response.DumpToString());
            return new FileInfo(path);
        }
    }
}