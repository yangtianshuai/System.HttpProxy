namespace System.HttpProxy
{
    /// <summary>
    /// 过滤器接口
    /// 创建人：YTS
    /// 创建时间：2019-3-4
    /// </summary>
    public interface IHttpFilter
    {
        /// <summary>
        /// 请求过滤
        /// </summary>
        /// <param name="client">HTTP客户端</param>
        void Request(HttpClient client);
        /// <summary>
        /// 响应过滤
        /// </summary>
        /// <param name="client">HTTP客户端</param>
        void Response(HttpClient client);
        /// <summary>
        /// 错误日志
        /// </summary>
        /// <param name="ex"></param>
        void Exception(Exception ex);
    }
}