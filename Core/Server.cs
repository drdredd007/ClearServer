using ClearServer.Core.Requester;
using System;
using System.Net;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Threading;

namespace ClearServer
{

    sealed class Server
    {
        HttpListener httpListener = null;
        Server()
        {
            httpListener = new HttpListener();
            httpListener.Prefixes.Add("https://itinder.online/");
            httpListener.Start();
            while (true)
            {
                var HttpContext = httpListener.GetContext();
                Thread thread = new Thread(new ParameterizedThreadStart(ClientThread));
                thread.Start(HttpContext);
            }
            
        }

        static void ClientThread(Object StateInfo)
        {
            new ClientHandler((HttpListenerContext)StateInfo);
        }

        ~Server()
        {
            if (httpListener != null)
            {
                httpListener.Stop();
            }
        }

        public static void Main(string[] args)
        {
            if (AppDomain.CurrentDomain.IsDefaultAppDomain())
            {
                Console.WriteLine("Switching another domain");
                new AppDomainSetup
                {
                    ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase
                };
                var current = AppDomain.CurrentDomain;
                var strongNames = new StrongName[0];
                var domain = AppDomain.CreateDomain(
                    "ClearServer", null,
                    current.SetupInformation, new PermissionSet(PermissionState.Unrestricted),
                    strongNames);
                domain.ExecuteAssembly(Assembly.GetExecutingAssembly().Location);
            }
            new Server();
        }
    }
}


