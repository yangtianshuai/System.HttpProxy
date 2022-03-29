namespace System.HttpProxy
{
    /// <summary>
    /// 路由特性
    /// 创建人：YTS
    /// 创建时间：2019-3-6
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RouteAttribute : Attribute
    {
        public RouteAttribute(string template)
        {
            _template = template;
        }
        private string _template;
        /// <summary>
        /// 路由模板
        /// </summary>
        public string Template
        {
            get
            {
                return _template;
            }
        }
    }
}