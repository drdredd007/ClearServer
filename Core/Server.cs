

using ClearServer.Core.Chat;
using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClearServer
{

    class Server
    {
        readonly TcpListener Listener;

        public Server(int Port)
        {
            Listener = new TcpListener(IPAddress.Any, Port);
            Listener.Start();

            while (true)
            {
                TcpClient Client = Listener.AcceptTcpClient();
                Thread Thread = new Thread(new ParameterizedThreadStart(ClientThread));
                Thread.Start(Client);
            }
        }

        static void ClientThread(Object StateInfo)
        {
            new Client((TcpClient)StateInfo);
        }

        ~Server()
        {
            if (Listener != null)
            {
                Listener.Stop();
            }
        }

        public static int Main()
        {
            if (AppDomain.CurrentDomain.IsDefaultAppDomain())
            {
                Console.WriteLine("Switching another domain");
                AppDomainSetup domainSetup = new AppDomainSetup();
                domainSetup.ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                var current = AppDomain.CurrentDomain;
                var strongNames = new StrongName[0];
                var domain = AppDomain.CreateDomain(
                    "ClearServer", null,
                    current.SetupInformation, new PermissionSet(PermissionState.Unrestricted),
                    strongNames);
                return domain.ExecuteAssembly(Assembly.GetExecutingAssembly().Location);
            }
            Application.EnableVisualStyles();
            //Task.Run(() => Application.Run(new Form1()));
            new Server(80);
            return 0;
        }

        static void StartingServer()
        {
            new Server(80);
        }
    }
}


