

using System;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Permissions;
using System.Security.Policy;
using System.Threading;
namespace ClearServer
{

    sealed class Server
    {
        readonly bool ServerRunning = true;
        readonly TcpListener sslListner;
        public static X509Certificate serverCertificate = null;
        Server()
        {
            serverCertificate = X509Certificate.CreateFromSignedFile(@"C:\ssl\itinder.online.crt");
            sslListner = new TcpListener(IPAddress.Any, 443);
            sslListner.Start();
            Console.WriteLine("Starting server.." + serverCertificate.Subject + "\n" + Assembly.GetExecutingAssembly().Location);
            while (ServerRunning)
            {
                TcpClient SslClient = sslListner.AcceptTcpClient();
                Thread SslThread = new Thread(new ParameterizedThreadStart(ClientThread));
                SslThread.Start(SslClient);
            }
            
        }
        static void ClientThread(Object StateInfo)
        {
            new Client((TcpClient)StateInfo);
        }

        ~Server()
        {
            if (sslListner != null)
            {
                sslListner.Stop();
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


