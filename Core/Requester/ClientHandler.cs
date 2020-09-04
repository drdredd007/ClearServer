using ClearServer.Core.UserController;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace ClearServer.Core.Requester
{
    public class ClientHandler
    {
        HttpListenerContext ClientContext;
        public string Message = "";
        public bool isMobile = false;
        public bool isAuth = false;
        private readonly byte[] buffer = null;
        private event Action<HttpListenerContext, ClientHandler> OnRead = RequestHandler.OnHandle;
        public ClientHandler(HttpListenerContext ClientContext)
        {
            this.ClientContext = ClientContext;
            try
            {
                buffer = new byte[ClientContext.Request.ContentLength64];
                ClientContext.Request.InputStream.BeginRead(buffer, 0, buffer.Length, ClientRead, ClientContext);
            }
            catch { return; }
        }
        private void ClientRead(IAsyncResult ar)
        {

            if (ar.IsCompleted)
            {
                Message = Encoding.UTF8.GetString(buffer);
                Message = Uri.UnescapeDataString(Message);
                //Console.WriteLine($"\n{DateTime.Now:g} Client IP:{ClientContext.Request.RemoteEndPoint}\n{ClientContext.Request.RawUrl}\n{Message}");
                isMobile = ClientContext.Request.Headers.AllKeys.Any(str => str == "ItinderMobile");             
                OnRead?.Invoke(ClientContext, this);
            }
        }
    }
}
