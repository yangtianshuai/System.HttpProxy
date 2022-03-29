namespace System.HttpProxy
{
    /// <summary>
    /// 服务无法解析
    /// 创建人：YTS
    /// 创建时间：2019-3-5
    /// </summary>
    internal class BadResult : ActionResult
    {
        private string response = "Bad Request";
        public BadResult(string _response)
        {
            this.ContentType = "text/html";
            response = _response;
        }
        
        public override string ToResponse()
        {
            return response;
        }
    }
}