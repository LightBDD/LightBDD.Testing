using System.Collections.Generic;

namespace LightBDD.Testing.Http
{
    public class RecordedHttpCallRepository
    {
        private readonly List<ITestableHttpResponse> _items = new List<ITestableHttpResponse>();

        public IEnumerable<ITestableHttpResponse> GetAll()
        {
            return _items;
        }

        public void Add(ITestableHttpResponse response)
        {
            _items.Add(response);
        }
    }
}