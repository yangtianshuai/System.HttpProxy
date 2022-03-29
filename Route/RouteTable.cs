namespace System.HttpProxy
{
    /// <summary>
    /// 路由表
    /// 创建人：YTS
    /// 创建时间：2019-3-5
    /// </summary>
    public class RouteTable
    {
        private RouteTable() { }
        /// <summary>
        /// 路由集合，存放所有自定义路由
        /// </summary>
        private static RouteCollection _instance = null;

        //单例模式
        public static RouteCollection Routes
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RouteCollection();
                }
                return _instance;
            }
        }
    }
}