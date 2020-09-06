using ClearServer.Core.Requester;
using System;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Threading;

namespace ClearServer
{
    sealed class Server
    {
        HttpListener _httpListener = null;
        Server()
        {
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add("https://itinder.online/");
            _httpListener.Start();
            
            while (true)
            {
                var httpContext = _httpListener.GetContext();
                Thread thread = new Thread(new ParameterizedThreadStart(ClientThread));
                thread.Start(httpContext);
            }
            
        }

        static void ClientThread(Object StateInfo)
        {
            new ClientHandler((HttpListenerContext)StateInfo);
        }

        ~Server()
        {
            if (_httpListener != null)
            {
                _httpListener.Stop();
            }
        }

        public static void Main(string[] args)
        {
            new Server();
        }
    }
}


