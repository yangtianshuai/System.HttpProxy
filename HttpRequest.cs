using System.Collections.Generic;

namespace System.HttpProxy
{
    /// <summary>
    /// HTTP请求
    /// 创建人：YTS
    /// 创建时间：2019-3-5
    /// </summary>
    public class HttpRequest
    {
        public HttpRequest()
        {
            Method = "";
            URL = "";
            Body = "";
        }
        public delegate void BeforeResolve();
        /// <summary>
        /// 开始处理之前
        /// </summary>
        public BeforeResolve BeforeResolveEvent;

        public delegate void DealCompleted(ActionResult result);
        /// <summary>
        /// 业务处理完成
        /// </summary>
        public DealCompleted CompletedEvent;

        /// <summary>
        /// 请求是否合法
        /// </summary>
        public bool IsValid { get; set; }
        /// <summary>
        /// 请求方法
        /// </summary>
        public string Method { get; set; }
        /// <summary>
        /// URL地址
        /// </summary>
        public string URL { get; set; }
        
        /// <summary>
        /// 头部集合
        /// </summary>
        public Dictionary<string, string> Headers { get; private set; }
        /// <summary>
        /// 主体
        /// </summary>
        public string Body { get; set; }

        public int ContentLength
        {
            get
            {
                try
                {
                    if (Headers.ContainsKey("Content-Length"))
                    {
                        return int.Parse(Headers["Content-Length"].Trim());
                    }
                }
                catch { }
                return 0;
            }
        }

        private HttpResponse _response;

        public void SetResponse(HttpResponse response)
        {
            _response = response;
            Headers = new Dictionary<string, string>();
        }

        public HttpResponse GetResponse()
        {
            return _response;
        }

        //数据解析
        public void Resolve(string data, Action<string> BodyResolve)
        {
            string[] _lines = data.Split(new string[] { "\r\n\r\n" }, StringSplitOptions.None);
            ResolveHeader(_lines[0]);
            if (IsValid && _lines.Length == 2)
            {
                if (BodyResolve != null)
                {
                    BodyResolve(_lines[1]);
                }
            }
        }

        private void ResolveHeader(string data)
        {
            string[] _lines = data.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            if (_lines.Length == 0)
            {
                IsValid = false;
            }
            string[] array = _lines[0].Split(' ');
            if (array.Length > 1)
            {
                Method = array[0].ToLower();
                URL = array[1].ToLower();
                if (HttpVerb.IsMethod(Method))
                {
                    IsValid = true;
                }
            }
            else
            {
                IsValid = false;
            }
            if (IsValid)
            {
                //收集头部
                for (int i = 1; i < _lines.Length; i++)
                {
                    var index = _lines[i].IndexOf(':');
                    if (index > 0)
                    {
                        Headers.Add(_lines[i].Substring(0, index)
                            , _lines[i].Substring(index + 1, _lines[i].Length - index - 1));
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        public void Action()
        {
            if (BeforeResolveEvent != null)
            {
                BeforeResolveEvent();
            }

            ActionResult result = null;

            if (IsValid)
            {
                //路由
                var router = new HttpRouter(URL, Body);
                if (router == null)
                {
                    _response.Status = ResponseStatus.NotFound;
                    result = new BadResult(_response.Status);
                }
                else
                {
                    if (router.Route != null)
                    {
                        //进行路由操作
                        router.Route.Router(this);
                    }
                    if (router.Controller != null)
                    {
                        //执行业务操作
                        result = router.Controller.Execute();
                        _response.Status = ResponseStatus.OK;
                    }
                    else
                    {
                        _response.Status = ResponseStatus.NotFound;
                        result = new BadResult(_response.Status);
                    }
                }
            }
            else
            {
                _response.Status = ResponseStatus.BadRequest;
                result = new BadResult(_response.Status);
            }

            if (CompletedEvent != null)
            {
                CompletedEvent(result);
            }
        }
    }
}