using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace System.HttpProxy
{
    /// <summary>
    /// HTTP服务
    /// 创建人：YTS
    /// 创建时间：2019-3-4
    /// </summary>
    public class HttpServer:IDisposable
    {        
        private Socket _serverSocket = null;
        private int _maxSize = 50;
        //public static ManualResetEvent allDone = new ManualResetEvent(false);

        private static int _port = 8655;

        public static int GetPort()
        {
            return _port;
        }

        public void Dispose()
        {
            if (_serverSocket != null)
            {
                _serverSocket.Close();
                _serverSocket = null;
            }
        }

        public void Run(int? port = null)
        {
            if (port != null) _port = port.Value;
            Running();
        }
        private void Running()
        {
            Dispose();
            _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _serverSocket.Bind(new IPEndPoint(IPAddress.Any, _port));         
            _serverSocket.Listen(100);
            _serverSocket.BeginAccept(new AsyncCallback(Accept), _serverSocket);
            //allDone.WaitOne();
        }

        private void Accept(IAsyncResult ar)
        {            
            try
            {
                //allDone.Set();

                Socket socket = ar.AsyncState as Socket;
                Socket client = socket.EndAccept(ar);                
                var request = new HttpClient(client);
                //记录请求
                //HttpClientCollection.Add(request);              
                request.Disposed += new HttpClient.Dispose(RemoveRequest);
                //权限鉴定
                request.Deal(_maxSize);   //处理             
                
                //allDone.Reset();
                socket.BeginAccept(new AsyncCallback(Accept), socket); 
                //allDone.WaitOne();               
            }
            catch(Exception ex)
            {
                //发生错误                 
            }
        }        

        private void RemoveRequest(string id)
        {
            //HttpClientCollection.Remove(id);
        }
    }
}