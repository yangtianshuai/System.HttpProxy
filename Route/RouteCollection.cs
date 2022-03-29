using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace System.HttpProxy
{
    /// <summary>
    /// 路由集合
    /// 创建人：YTS
    /// 创建时间：2019-3-5
    /// </summary>
    public class RouteCollection
    {
        private Dictionary<string, RouteBase> maps = null;

        public RouteBase this[string url]
        {
            get
            {
                if (string.IsNullOrEmpty(url))
                {
                    return null;
                }
                RouteBase result;
                if (this.maps.TryGetValue(url, out result))
                {
                    return result;
                }
                return null;
            }
        }

        public List<string> GetKeys()
        {          
            return new List<string>(maps.Keys);
        }

        public RouteBase Match(string url)
        {
            var keys = GetKeys();
            foreach (var key in keys)
            {
                if (Regex.IsMatch(url, key + "$", RegexOptions.IgnoreCase))
                {
                    return this.maps[key];
                }
            }
            
            return null;
        }

        public RouteCollection()
        {
            maps = new Dictionary<string, RouteBase>();             
        }

        public void Add(RouteBase route)
        {
            if (!maps.ContainsKey(route.Url))
            {
                maps.Add(route.Url, route);
            }
        }

        public void Clear()
        {
            maps.Clear();
        }
    }
}