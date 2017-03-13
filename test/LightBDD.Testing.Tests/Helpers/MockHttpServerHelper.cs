using System.Threading;

namespace LightBDD.Testing.Tests.Helpers
{
    internal class MockHttpServerHelper
    {
        private static int _port = 39084;
        public static int GetNextPort()
        {
            return Interlocked.Increment(ref _port);
        }
    }
}
