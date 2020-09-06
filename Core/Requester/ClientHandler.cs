using ClearServer.Core.WebSockets;
using System;
using System.Linq;
using System.Net;
using System.Text;
using ClearServer.Core.UserController;

namespace ClearServer.Core.Requester
{
    public class ClientHandler
    {
        HttpListenerContext _clientContext;
        public string Message = "";
        public bool IsMobile = false;
        public bool IsAuth = false;
        public User CurrentUser = null;
        private readonly byte[] _buffer = null;
        private readonly DatabaseWorker _databaseWorker = null;
        private event Action<HttpListenerContext, ClientHandler> OnRead = RequestHandler.OnHandle;
        public ClientHandler(HttpListenerContext ClientContext)
        {
            _clientContext = ClientContext;
            _databaseWorker = new DatabaseWorker();
            try
            {
                _buffer = new byte[ClientContext.Request.ContentLength64];
                ClientContext.Request.InputStream.BeginRead(_buffer, 0, _buffer.Length, ClientRead, ClientContext);
            }
            catch { return; }
        }
        private void ClientRead(IAsyncResult ar)
        {

            if (ar.IsCompleted)
            {
                Message = Encoding.UTF8.GetString(_buffer);
                Message = Uri.UnescapeDataString(Message);
                Console.WriteLine($"\n{DateTime.Now:g} Client IP:{_clientContext.Request.RemoteEndPoint}\n{_clientContext.Request.HttpMethod} {_clientContext.Request.RawUrl}\n{_clientContext.Request.Headers}\n{Message}");
                IsMobile = _clientContext.Request.Headers.AllKeys.Any(str => str == "ItinderMobile");
                if (_clientContext.Request.IsWebSocketRequest)
                {
                    Console.WriteLine("try to connect");
                    ChatHandler.ChatConnection(_clientContext);
                }

                CurrentUser = GetUser();
                OnRead?.Invoke(_clientContext, this);
            }
        }

        private User GetUser()
        {
            if (_clientContext.Request.Cookies.Count <= 0) return null;
            User user = null;
            foreach (Cookie item in _clientContext.Request.Cookies)
            {
                user = _databaseWorker.CookieValidate(item.Value);
                IsAuth = (user != null);
                        
            }
            return user;
        }
    }
}
