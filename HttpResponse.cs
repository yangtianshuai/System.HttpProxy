using System.Collections.Generic;

namespace System.HttpProxy
{
    /// <summary>
    /// HTTP响应
    /// 创建人：YTS
    /// 创建时间：2019-3-4
    /// </summary>
    public class HttpResponse
    {
        public delegate void Action(ActionResult Result);
        /// <summary>
        /// 进行响应操作
        /// </summary>
        public Action ActionEvent;
        /// <summary>
        /// 返回状态码
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 头部集合
        /// </summary>
        public Dictionary<string, string> Headers { get; private set; }

        private ActionResult result;
        /// <summary>
        /// 返回结果
        /// </summary>
        public ActionResult Result
        {
            get
            {
                return result;
            }
            set
            {
                result = value;
                if (ActionEvent != null)
                {
                    ActionEvent(result);
                }
            }
        }

        public string StatusLine
        {
            get
            {
                return string.Format("HTTP/1.1 {0}", Status);
            }
        }

        public HttpResponse()
        {
            Headers = new Dictionary<string, string>();
        }

    }
}