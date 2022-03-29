using System.Collections.Generic;

namespace System.HttpProxy
{
    public class HttpClientCollection
    {
        private volatile static Dictionary<string, HttpClient> _requests = new Dictionary<string, HttpClient>();
        private readonly static object _lock = new object();

        private HttpClientCollection()
        {
            _requests.Clear();
        }

        public static void Add(HttpClient client)
        {
            lock (_lock)
            {
                _requests.Add(client.ID, client);
            }
        }

        public static void Remove(string id)
        {
            lock (_lock)
            {
                if (_requests.ContainsKey(id))
                {
                    _requests.Remove(id);
                }
            }
        }

    }
}