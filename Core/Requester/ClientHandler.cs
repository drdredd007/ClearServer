using ClearServer.Core.WebSockets;
using System;
using System.Linq;
using System.Net;
using System.Text;
using ClearServerCore.Core.Database;
using ClearServerCore.Core;

namespace ClearServer.Core.Requester
{
    public class ClientHandler
    {
        HttpListenerContext _clientContext;
        public string Message = "";
        public bool IsMobile = false;
        public bool IsAuth = false;
        public User CurrentUser = null;
        public readonly byte[] _buffer = null;
        private readonly DatabaseWorker _databaseWorker;
        private event Action<HttpListenerContext, ClientHandler> OnRead = RequestHandler.OnHandle;
        public ClientHandler(HttpListenerContext ClientContext)
        {
            _databaseWorker = DatabaseWorker.GetInstance();
            _clientContext = ClientContext;
            try
            {
                _buffer = new byte[_clientContext.Request.ContentLength64];
                ClientContext.Request.InputStream.BeginRead(_buffer, 0, _buffer.Length, ClientRead, null);
            }
            catch { return; }
        }
        private void ClientRead(IAsyncResult ar)
        {

            if (ar.IsCompleted)
            {
                Message = Encoding.UTF8.GetString(_buffer);
                Message = Uri.UnescapeDataString(Message);
                Console.WriteLine($"\n{DateTime.Now:g} Client IP:{_clientContext.Request.RemoteEndPoint}\n{_clientContext.Request.HttpMethod} {_clientContext.Request.RawUrl}\n{_clientContext.Request.Headers}\n");
                IsMobile = _clientContext.Request.Headers["ItinderMobile"] != null;
                CurrentUser = GetUser();
                if (IsAuth && _clientContext.Request.IsWebSocketRequest)
                {
                    ChatHandler.ChatConnection(_clientContext);
                }
                OnRead?.Invoke(_clientContext, this);

            }
        }

        private User GetUser()
        {
            User user = null;
            switch (_clientContext.Request)
            {
                case { } request when request.Headers["UserKey"] != null:
                    user = _databaseWorker.CookieValidate(request.Headers["UserKey"]);
                    break;
                case { } request when request.Cookies["User"] != null:
                    user = _databaseWorker.CookieValidate(request.Cookies["User"].Value);
                    break;
            }
            IsAuth = (user != null);
            return user;
        }
    }
}
