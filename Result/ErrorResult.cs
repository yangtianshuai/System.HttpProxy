namespace System.HttpProxy
{
    /// <summary>
    /// 服务器错误
    /// 创建人：YTS
    /// 创建时间：2019-3-6
    /// </summary>
    internal class ErrorResult : ActionResult
    {
        private string _error;

        public ErrorResult(string error)
        {
            _error = error;
            this.ContentType = "text/html";
        }

        public override string ToResponse()
        {
            return _error;
        }
    }
}