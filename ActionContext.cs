using System.Reflection;

namespace System.HttpProxy
{
    /// <summary>
    /// 执行上下文
    /// 创建人：YTS
    /// 创建时间：2019-3-5
    /// </summary>
    public class ActionContext
    {        
        /// <summary>
        /// 方法信息
        /// </summary>
        public MethodInfo Method { get; set; }
        /// <summary>
        /// 参数值
        /// </summary>
        public object[] Values { get; set; }
    }
}