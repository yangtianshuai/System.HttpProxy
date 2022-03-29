using System.Text;

namespace System.HttpProxy
{
    /// <summary>
    /// 执行结果
    /// 创建人：YTS
    /// 创建时间：2019-3-5
    /// </summary>
    public abstract class ActionResult
    {
        /// <summary>
        /// 主体内容数据类型
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// 编码方式
        /// </summary>
        public Encoding Encoding { get; set; } = Encoding.UTF8;
        /// <summary>
        /// 输出
        /// </summary>
        /// <returns></returns>
        public abstract string ToResponse();
       
    }
}