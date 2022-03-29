namespace System.HttpProxy
{
    /// <summary>
    /// API控制器基类
    /// 创建人：YTS
    /// 创建时间：2019-3-5
    /// </summary>
    public class ApiController : IController
    {
        public HttpRequest Request { get; internal set; }
        public ActionContext Context { get; set; }
        public virtual ActionResult Execute()
        {                 
            return (ActionResult)Context.Method.Invoke(this, Context.Values);
        }
    }
}