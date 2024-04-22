using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace System.HttpProxy
{
    /// <summary>
    /// HTTP客户端
    /// 创建人：YTS
    /// 创建时间：2019-3-4
    /// </summary>
    public class HttpClient : IDisposable
    {
        public delegate void Dispose(string id);
        public Dispose Disposed;

        private Socket _socket;
        private string _id;
        public string ID
        {
            get
            {
                return _id;
            }
        }
        private HttpRequest _request;
        /// <summary>
        /// HTTP请求
        /// </summary>
        public HttpRequest Request
        {
            get
            {
                return _request;
            }
        }
        private HttpResponse _response;
        /// <summary>
        /// HTTP响应
        /// </summary>
        public HttpResponse Response
        {
            get
            {
                return _response;
            }
            set
            {
                _response = value;
                if (_response.Result != null)
                {
                    this.Send(_response.Result);
                }
            }
        }

        public string GetRemoteIp()
        {
            try
            {
                return ((IPEndPoint)_socket.RemoteEndPoint).Address.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        public HttpClient(Socket socket)
        {
            _socket = socket;
            _id = System.Guid.NewGuid().ToString();
            Response = new HttpResponse();
        }

        public void Deal(int size)
        {
            _request = new HttpRequest();
            _request.SetResponse(_response);
            _request.BeforeResolveEvent += new HttpRequest.BeforeResolve(StartProcess);
            _request.CompletedEvent += new HttpRequest.DealCompleted(Send);

            var buffer = new HttpBuffer(size);
            _socket.BeginReceive(buffer.Buffer, 0, buffer.Buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), buffer);
        }

        private void ReceiveCallback(IAsyncResult result)
        {
            try
            {
                var buffer = (HttpBuffer)result.AsyncState;
                int receive = _socket.EndReceive(result);
                if (receive > 0)
                {

                    if (_request.URL.Length == 0)
                    {
                        string data = "";
                        if (buffer.GetStream().Length > 0)
                        {
                            //继续追加头部信息
                            buffer.Stream(receive);
                            data = Encoding.UTF8.GetString(buffer.GetStream());
                        }
                        else
                        {
                            //获取头部
                            data = Encoding.UTF8.GetString(buffer.Buffer, 0, receive);
                        }

                        _request.Resolve(data, body =>
                        {
                            buffer.Clear();
                            buffer.Buffer = Encoding.UTF8.GetBytes(body);
                            buffer.Stream();
                        });

                        if (_request.URL.Length == 0 && buffer.GetStream().Length == 0)
                        {
                            buffer.Stream(receive);
                        }
                    }
                    else
                    {
                        buffer.Stream(receive);
                    }
                    if (_request.URL.Length > 0 && (!_request.IsValid ||
                          buffer.GetStream().Length >= _request.ContentLength))
                    {
                        if (_request.Body.Length == 0)
                        {
                            _request.Body = Encoding.UTF8.GetString(buffer.GetStream());
                        }
                        buffer.Dispose();
                        _request.Action();
                    }
                    else
                    {
                        buffer.Clear();
                        _socket.BeginReceive(buffer.Buffer, 0, buffer.Buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), buffer);
                    }

                }
                else
                {
                    _response.Status = ResponseStatus.Forbidden;
                    //是否显示服务器端错误，默认显示
                    Send(new ErrorResult("Forbidden"));
                }
            }
            catch (Exception ex)
            {
                FilterCollection.Exception(ex);
                _response.Status = ResponseStatus.InternalError;
                //是否显示服务器端错误，默认显示
                Send(new ErrorResult(ex.Message));
            }
        }

        /// <summary>
        /// 获取IP地址
        /// </summary>
        /// <returns></returns>
        public string GetIP()
        {
            return ((IPEndPoint)_socket.RemoteEndPoint).Address.ToString();
        }

        public bool Exist()
        {
            return this._socket != null && this._socket.Connected;
        }

        //业务开始处理
        private void StartProcess()
        {
            FilterCollection.Request(this);
        }

        private void Send(ActionResult result)
        {
            if (_request == null)
            {
                return;
            }
            try
            {
                _request.CompletedEvent -= new HttpRequest.DealCompleted(Send);
                //发送状态行
                var headerText = _response.StatusLine + "\r\n";

                if (result != null)
                {
                    if (_request.Headers.ContainsKey("Origin"))
                    {
                        //跨域设置
                        _response.Headers.Add("Access-Control-Allow-Origin", _request.Headers["Origin"]);
                        _response.Headers.Add("Access-Control-Allow-Credentials", "true");
                        _response.Headers.Add("Vary", "Origin");
                    }

                    if (result.ContentType != null && result.ContentType.Length > 0)
                    {
                        //发送应答头
                        if (_response.Headers.ContainsKey("Content-Type"))
                        {
                            _response.Headers.Remove("Content-Type");
                        }
                        _response.Headers.Add("Content-Type", $"{result.ContentType}; charset={result.Encoding.BodyName}");
                    }

                    //输出HTTP返回
                    byte[] send_byte = null;
                    var content = result.ToResponse();
                    if (content != null && content.Length > 0)
                    {
                        //输出HTTP返回
                        send_byte = result.Encoding.GetBytes(content);
                        //设置主体长度
                        _response.Headers.Add("Content-Length", send_byte.Length.ToString());
                    }
                    else
                    {
                        _response.Headers.Add("Content-Length", "0");
                    }

                    _response.Headers.Add("Connection", "close");

                    var keys = new List<string>(_response.Headers.Keys);

                    for (int i = 0; i < keys.Count; i++)
                    {
                        headerText += $"{keys[i]}: {_response.Headers[keys[i]]}";
                        if (i < keys.Count - 1)
                        {
                            headerText += "\r\n";
                        }
                    }
                   
                    if (send_byte == null)
                    {
                        _socket.Send(Encoding.UTF8.GetBytes(headerText));
                        return;
                    }

                    byte[] buffer = Encoding.UTF8.GetBytes(headerText);

                    var buffer2 = new byte[buffer.Length + 4 + send_byte.Length];
                    buffer.CopyTo(buffer2, 0);
                    (new byte[] { (byte)'\r', (byte)'\n', (byte)'\r', (byte)'\n' }).CopyTo(buffer2, buffer.Length);
                    //发送空行
                    //_socket.Send(new byte[] { (byte)'\r', (byte)'\n', (byte)'\r', (byte)'\n' });
                    //_socket.Send(send_byte);
                    send_byte.CopyTo(buffer2, buffer.Length + 4);
                    _socket.Send(buffer2);
                    FilterCollection.Response(this);
                }
                else
                {
                    _socket.Send(Encoding.UTF8.GetBytes(headerText));
                }
                _socket.Shutdown(SocketShutdown.Both);
                //关闭客户端连接
                _socket.Close();
            }
            catch (Exception ex)
            {
                FilterCollection.Exception(ex);              
                if (_socket != null)
                {
                    _socket.Close();
                }
            }
            if (Disposed != null)
            {
                Disposed(_id);
            }
        }

        //private void SendAsync(byte[] buffer)
        //{
        //    handler.BeginSend(buffer, 0, buffer.Length, 0,new AsyncCallback(SendCallback), _socket);
        //}

        private void SendCallback(IAsyncResult ar)
        {                     
            Socket handler = (Socket)ar.AsyncState;  
            handler.EndSend(ar);          
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();            
        }

        void IDisposable.Dispose()
        {
            _socket = null;
            if (_request != null)
            {
                _request.CompletedEvent -= new HttpRequest.DealCompleted(Send);
            }
            _request = null;
        }
    }
}