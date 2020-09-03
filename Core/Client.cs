using System;
using System.IO;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using RazorEngine;
using Encoding = System.Text.Encoding;
using RazorEngine.Templating;
using System.Security.Cryptography;
using System.Net.Security;
using System.Net;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using ClearServer.Core.Requester;

namespace ClearServer
{
    public class Client
    {
        static readonly string MagicKey = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
        public Client(TcpClient Client)
        {
            SslStream SSlClientStream = new SslStream(Client.GetStream(), false);
            try
            {
                SSlClientStream.AuthenticateAsServer(Server.serverCertificate, clientCertificateRequired: false, checkCertificateRevocation: true);
            }
            catch (Exception e)
            {
                Console.WriteLine(
                    "---------------------------------------------------------------------\n" +
                    $"|{DateTime.Now:g}\n|------------\n|{Client.Client.RemoteEndPoint}\n|------------\n|Exception: {e.Message}\n|------------\n|Authentication failed - closing the connection.\n" +
                    "---------------------------------------------------------------------\n");
                SSlClientStream.Close();
                Client.Close();
            }
            new RequestContext(SSlClientStream, Client);
        }

    }
}
