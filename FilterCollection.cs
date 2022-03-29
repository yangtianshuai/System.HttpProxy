using System.Collections.Generic;

namespace System.HttpProxy
{
    /// <summary>
    /// 过滤器容器
    /// 创建人：YTS
    /// 创建时间：2019-3-6
    /// </summary>
    public class FilterCollection
    {
        private static List<IHttpFilter> _filters = new List<IHttpFilter>();

        public static void Set(IHttpFilter[] filters)
        {
            _filters = new List<IHttpFilter>(filters);
        }

        public static void Add(IHttpFilter filter)
        {
            if (_filters == null)
            {
                _filters = new List<IHttpFilter>();
            }
            if (!_filters.Contains(filter))
            {
                _filters.Add(filter);
            }
        }

        public static void Request(HttpClient http)
        {
            foreach (var filter in _filters)
            {
                if (http.Exist())
                {
                    try
                    {
                        filter.Request(http);
                    }
                    catch
                    { }
                }
            }
        }

        public static void Response(HttpClient http)
        {
            foreach (var filter in _filters)
            {
                if (http.Exist())
                {
                    try
                    {
                        filter.Response(http);
                    }
                    catch
                    { }
                }
            }
        }

        public static void Exception(Exception ex)
        {
            if (_filters == null)
            {
                return;
            }
            foreach (var filter in _filters)
            {
                try
                {
                    filter.Exception(ex);
                }
                catch
                { }
            }
        }
    }
}