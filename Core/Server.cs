using ClearServer.Core.Requester;
using ClearServerCore.Core.RazorController;
using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace ClearServer
{
    sealed class Server
    {
        HttpListener _httpListener = null;
        Server()
        {
            RazorEngine.Init();
            DatabaseWorker.GetInstance();
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add("https://itinder.online/");
            _httpListener.Prefixes.Add("http://itinder.online/");
            _httpListener.Start();
            Console.WriteLine("Server started");
            foreach (var item in _httpListener.Prefixes)
            {
                Console.WriteLine(item);
            }
            
            while (true)
            {
                var httpContext = _httpListener.GetContextAsync().Result;
                Thread thread = new Thread(ClientThread);
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


