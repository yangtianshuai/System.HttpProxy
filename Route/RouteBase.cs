namespace System.HttpProxy
{
    /// <summary>
    /// 路由基类
    /// 创建人：YTS
    /// 创建时间：2019-3-5
    /// </summary>
    public abstract class RouteBase
    {
        public delegate void RouteCompleted(IController controller);
        /// <summary>
        /// 路由处理完成事件
        /// </summary>
        public RouteCompleted RouteCompletedEvent;
        /// <summary>
        /// 原始URL
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// 执行路由
        /// </summary>
        /// <param name="request"></param>
        public abstract void Router(HttpRequest request);
    }
}