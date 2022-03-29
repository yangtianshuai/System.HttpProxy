namespace System.HttpProxy
{
    /// <summary>
    /// HTTP请求方法
    /// 创建人：YTS
    /// 创建时间：2019-3-4
    /// </summary>
    public class HttpVerb
    {
        public const string Get = "get";
        public const string Post = "post";
        public const string Delete = "delete";
        public const string Put = "put";

        public static bool IsMethod(string method)
        {
            return method == Get || method == Post;
        }
    }
}