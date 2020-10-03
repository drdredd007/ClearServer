using ClearServer.Core.WebSockets;
using System;
using System.Linq;
using System.Net;
using System.Text;
using ClearServerCore.Core.Database;
using ClearServerCore.Core;
using System.IO;

namespace ClearServer.Core.Requester
{
    public class ClientHandler
    {
        HttpListenerContext _clientContext;
        public string Message = "";
        public bool IsMobile = false;
        public bool IsAuth = false;
        public User CurrentUser = null;
        public byte[] _buffer = null;
        private readonly DatabaseWorker _databaseWorker;
        private event Action<HttpListenerContext, ClientHandler> OnRead = RequestHandler.OnHandle;
        public ClientHandler(HttpListenerContext ClientContext)
        {
            _databaseWorker = DatabaseWorker.GetInstance();
            _clientContext = ClientContext;
            try
            {
                ClientRead();
            }
            catch { return; }
        }
        private void ClientRead()
        {
            var reader = new BinaryReader(_clientContext.Request.InputStream);
            _buffer = reader.ReadBytes(Convert.ToInt32(_clientContext.Request.ContentLength64));


            Message = Encoding.UTF8.GetString(_buffer);
            Message = Uri.UnescapeDataString(Message);


            Console.WriteLine($"\n{DateTime.Now:g} Client IP:{_clientContext.Request.RemoteEndPoint}\n{_clientContext.Request.HttpMethod} {_clientContext.Request.RawUrl}\n{_clientContext.Request.Headers}\n");


            IsMobile = _clientContext.Request.Headers["ItinderMobile"] != null;


            CurrentUser = GetUser();
            IsAuth = (CurrentUser != null);



            if (IsAuth && _clientContext.Request.IsWebSocketRequest)
            {
              new ChatHandler().ChatConnection(_clientContext, this);
            }
            OnRead?.Invoke(_clientContext, this);

        }

        private User GetUser() => _clientContext.Request switch
        {
            var Request when Request.Headers["UserKey"] != null => _databaseWorker.CookieValidate(Request.Headers["UserKey"]),
            var Request when Request.Cookies["User"] != null => _databaseWorker.CookieValidate(Request.Cookies["User"].Value),
            _ => null
        };
    }
}
