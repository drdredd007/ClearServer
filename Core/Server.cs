

using ClearServer.Core.Chat;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ClearServer
{

    sealed class Server
    {
        readonly TcpListener Listener;
        public static X509Certificate serverCertificate = null;
        public Server()
        {
            serverCertificate = X509Certificate.CreateFromSignedFile(@"C:\ssl\itinder.online.crt");
            
            Listener = new TcpListener(IPAddress.Any, 443);
            Listener.Start();
            Console.WriteLine("Starting server.." + serverCertificate.Subject);
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

        public static int Main(string[] args)
        {
            //if (AppDomain.CurrentDomain.IsDefaultAppDomain())
            //{
            //    Console.WriteLine("Switching another domain");
            //    AppDomainSetup domainSetup = new AppDomainSetup();
            //    domainSetup.ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            //    var current = AppDomain.CurrentDomain;
            //    var strongNames = new StrongName[0];
            //    var domain = AppDomain.CreateDomain(
            //        "ClearServer", null,
            //        current.SetupInformation, new PermissionSet(PermissionState.Unrestricted),
            //        strongNames);
            //    return domain.ExecuteAssembly(Assembly.GetExecutingAssembly().Location);
            //}
            Application.EnableVisualStyles();//Task.Run(() => Application.Run(new Form1()));
            new Server();
            return 0;
        }
    }
}


