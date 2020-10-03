using ClearServer.Core.Requester;
using ClearServerCore.Core.RazorController;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using JavaScriptEngineSwitcher.Core;
using React;
using JavaScriptEngineSwitcher.V8;

namespace ClearServer
{
    sealed class Server
    {
        HttpListener _httpListener = null;

        Server()
        {
            //RazorEngine.Init();
            try
            {
                ReactSiteConfiguration.Configuration.AddScript("~/View/React/script.js");
                var engineSwitcher = JsEngineSwitcher.Current;
                engineSwitcher.EngineFactories.AddV8();
                engineSwitcher.DefaultEngineName = V8JsEngine.EngineName;
                Console.WriteLine(ReactEnvironment.Current.Version);


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

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
                try
                {
                    var httpContext = _httpListener.GetContextAsync().Result;
                    Thread thread = new Thread(ClientThread);
                    thread.Start(httpContext);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
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

    public class JsFactory
    {
        public static void Configure(JsEngineSwitcher switcher)
        {
           


        }
    }
}


