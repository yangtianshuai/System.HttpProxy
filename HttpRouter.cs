using System.Text.RegularExpressions;

namespace System.HttpProxy
{
    /// <summary>
    /// HTTP路由
    /// 创建人：YTS
    /// 创建时间：2019-3-4
    /// </summary>
    public class HttpRouter
    {
        public IController Controller { get; set; }

        public RouteBase Route { get; set; }

        public HttpRouter(string url) : this(url, null)
        {
        }

        public HttpRouter(string url, string body)
        {
            int index = url.IndexOf("?");
            if (index >= 0)
            {
                url = url.Substring(0, index);
            }
            Route = RouteTable.Routes.Match(url);
            if (Route == null)
            {

            }
            if (Route != null)
            {
                Route.RouteCompletedEvent += new RouteBase.RouteCompleted(SetController);
            }
        }

        private void SetController(IController controller)
        {
            this.Controller = controller;
        }
    }
}