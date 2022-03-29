namespace System.HttpProxy
{
    /// <summary>
    /// 控制器基类
    /// 创建人：YTS
    /// 创建时间：2019-3-5
    /// </summary>
    public interface IController
    {
        /// <summary>
        /// 请求执行
        /// </summary>
        /// <returns></returns>
        ActionResult Execute();
    }
}