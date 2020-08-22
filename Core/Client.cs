using ClearServer.Core.Parser;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using RazorEngine;
using Encoding = System.Text.Encoding;
using System.Dynamic;
using RazorEngine.Templating;
using System.Web;
using System.Security.Cryptography;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Authentication;

namespace ClearServer
{
    class Client
    {

        public Client(TcpClient Client)
        {
            SslStream ClientStream = new SslStream(Client.GetStream(), false);
            try
            {
                Console.WriteLine("try to connect");
                ClientStream.AuthenticateAsServer(Server.serverCertificate, clientCertificateRequired: false, checkCertificateRevocation: true);

                ////DisplaySecurityLevel(ClientStream);
                ////DisplaySecurityServices(ClientStream);
                ////DisplayCertificateInformation(ClientStream);
                ////DisplayStreamProperties(ClientStream);
                //string sslData = ReadMessage(ClientStream);


                //Console.WriteLine(sslData);

                //byte[] message = Encoding.UTF8.GetBytes("<html>Hello from server</html>");
                //ClientStream.Write(message);
                ////ClientStream.ReadTimeout = 0;
                ////ClientStream.WriteTimeout = 0;
                //ClientStream.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: {0}", e.Message);
                if (e.InnerException != null)
                {
                    Console.WriteLine("Inner exception: {0}", e.InnerException.Message);
                }
                Console.WriteLine("Authentication failed - closing the connection.");
                ClientStream.Close();
                Client.Close();
                return;
            }
            string Message = "";
            byte[] Buffer = new byte[1024];
            int Count;
            try
            {
                while ((Count = ClientStream.Read(Buffer, 0, Buffer.Length)) > 0)
                {
                    Message += Encoding.ASCII.GetString(Buffer, 0, Count);

                    if (Message.IndexOf("\r\n\r\n") >= 0 || Message.Length > 4096)
                    {
                        new Parser(Message, ClientStream);
                        //Console.WriteLine(Message);
                        Console.WriteLine(Uri.UnescapeDataString(Message));

                        Match socketMatch = Regex.Match(Message, @"Sec-WebSocket-Key: (.*)");

                        Console.WriteLine($"!{socketMatch.Groups[1].ToString()}!");

                        break;
                    }

                }
            }
            catch (Exception)
            {

            }
            //testRazor(ClientStream);
            Match MethodMatch = Regex.Match(Message, @"^(GET|POST)");

            if (MethodMatch != Match.Empty)
            {

                switch (MethodMatch.Groups[1].Value)
                {
                    case "GET":
                        Response(ClientStream, Message);
                        break;
                    case "POST":
                        break;
                }
                ClientStream.Close();
            }

        }

        private string ReadMessage(SslStream clientStream)
        {
            byte[] buffer = new byte[2048];
            StringBuilder messageData = new StringBuilder();
            int bytes = -1;
            do
            {
                bytes = clientStream.Read(buffer, 0, buffer.Length);
                Decoder decoder = Encoding.UTF8.GetDecoder();
                char[] chars = new char[decoder.GetCharCount(buffer,0, bytes)];
                decoder.GetChars(buffer, 0, bytes, chars, 0);
                messageData.Append(chars);
                if (messageData.ToString().IndexOf("<EOF>") != -1)
                {
                    break;
                }

            } while (bytes != 0);
            return messageData.ToString();
        }

        private void DisplayStreamProperties(SslStream stream)
        {
            Console.WriteLine("Can read: {0}, write {1}", stream.CanRead, stream.CanWrite);
            Console.WriteLine("Can timeout: {0}", stream.CanTimeout);
        }

        private void DisplayCertificateInformation(SslStream stream)
        {
            Console.WriteLine("Certificate revocation list checked: {0}", stream.CheckCertRevocationStatus);

            X509Certificate localCertificate = stream.LocalCertificate;
            if (stream.LocalCertificate != null)
            {
                Console.WriteLine("Local cert was issued to {0} and is valid from {1} until {2}.",
                    localCertificate.Subject,
                    localCertificate.GetEffectiveDateString(),
                    localCertificate.GetExpirationDateString());
            }
            else
            {
                Console.WriteLine("Local certificate is null.");
            }
            // Display the properties of the client's certificate.
            X509Certificate remoteCertificate = stream.RemoteCertificate;
            if (stream.RemoteCertificate != null)
            {
                Console.WriteLine("Remote cert was issued to {0} and is valid from {1} until {2}.",
                    remoteCertificate.Subject,
                    remoteCertificate.GetEffectiveDateString(),
                    remoteCertificate.GetExpirationDateString());
            }
            else
            {
                Console.WriteLine("Remote certificate is null.");
            }
        }

        private void DisplaySecurityServices(SslStream stream)
        {
            Console.WriteLine("Is authenticated: {0} as server? {1}", stream.IsAuthenticated, stream.IsServer);
            Console.WriteLine("IsSigned: {0}", stream.IsSigned);
            Console.WriteLine("Is Encrypted: {0}", stream.IsEncrypted);
        }

        private void DisplaySecurityLevel(SslStream stream)
        {
            Console.WriteLine("Cipher: {0} strength {1}", stream.CipherAlgorithm, stream.CipherStrength);
            Console.WriteLine("Hash: {0} strength {1}", stream.HashAlgorithm, stream.HashStrength);
            Console.WriteLine("Key exchange: {0} strength {1}", stream.KeyExchangeAlgorithm, stream.KeyExchangeStrength);
            Console.WriteLine("Protocol: {0}", stream.SslProtocol);
        }

        public static void Response(SslStream ClientStream, string Message, string cookie = "")
        {
            Match ReqMatch = Regex.Match(Message, @"^\w+\s+([^\s\?]+)[^\s]*\s+HTTP/.*|");
            if (ReqMatch == Match.Empty)
            {
                ErrorWorker.SendError(ClientStream, 400);
                return;
            }
            string RequestUri = ReqMatch.Groups[1].Value;
            RequestUri = Uri.UnescapeDataString(RequestUri);
            if (RequestUri.IndexOf("..") >= 0)
            {
                ErrorWorker.SendError(ClientStream, 400);
                return;
            }
            if (RequestUri.EndsWith("/"))
            {
                RequestUri += "index.html";
            }

            string FilePath = $"C:/Users/drdre/source/repos/ClearServer/View/{RequestUri}";

            if (!File.Exists(FilePath))
            {
                ErrorWorker.SendError(ClientStream, 404);
                return;
            }

            string Extension = RequestUri.Substring(RequestUri.LastIndexOf('.'));
            FileStream FS;

            if (!string.IsNullOrEmpty(cookie))
            {
                cookie = $"\nSet-Cookie: {cookie}: Max-Age;";
            }
            FS = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);


            string Headers = $"HTTP/1.1 {ExtensionCast(Extension, Message,FS)}\n\n";
            Console.WriteLine(Headers);


            byte[] HeadersBuffer = Encoding.ASCII.GetBytes(Headers);

            try
            {
                
                if (Extension == ".cshtml")
                {
                    //Отсюда выполнять запросы по кастомным страницам Выкинуть в отдельный метод, создать параметры загрузки.
                    string template = File.ReadAllText(FilePath);
                    var model = new { Name = "Test name from code", City = "Test city also from code" };
                    string result = Engine.Razor.RunCompile(template, "key", null, model);
                    var buffer = Encoding.UTF8.GetBytes(result);
                    ClientStream.Write(buffer, 0, buffer.Length);
                }
                else
                {
                    int Count;
                    byte[] responseBuffer = new byte[FS.Length];
                    ClientStream.Write(HeadersBuffer, 0, HeadersBuffer.Length);
                    while (FS.Position < FS.Length)
                    {
                        Count = FS.Read(responseBuffer, 0, responseBuffer.Length);
                        ClientStream.Write(responseBuffer, 0, Count);
                    }
                }
                
            }
            catch (Exception)
            {

            }

            FS.Close();

        }

        void testRazor(NetworkStream ClientStream)
        {


            string template = File.ReadAllText(@"D:\Web\DreamWeaver_proj\testcode.cshtml");
            var model = new { Name = "Matt" };
            string result = Engine.Razor.RunCompile(template, "key", null, model);
            var buffer = Encoding.UTF8.GetBytes(result);
            ClientStream.Write(buffer, 0, buffer.Length);
            ClientStream.Close();
        }

        private static string ExtensionCast(string Extension, string Message, FileStream FS)
        {
            //Match socketMatch = Regex.Match(Message, @"Sec-WebSocket-Key: (.*)");
            //var socketKey = socketMatch.Groups[1].Value;
            //string ClientKey = socketKey;
            //string GUID = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
            //var conc = ClientKey + GUID;
            //var buffer = Encoding.UTF8.GetBytes(conc);
            //var hash1 = SHA1.Create().ComputeHash(buffer);
            //var AcceptKey = Convert.ToBase64String(hash1);
            //Console.WriteLine($"!{ClientKey}!");
            string Header = "";
            string ContentType = "";
            switch (Extension)
            {
                case ".htm":
                case ".html":
                case ".cshtml":
                    ContentType = "text/html";
                    break;
                case ".css":
                    ContentType = "text/css";
                    break;
                case ".js":
                    ContentType = "text/javascript";
                    break;
                case ".jpg":
                    ContentType = "image/jpeg";
                    break;
                case ".jpeg":
                case ".png":
                case ".gif":
                    ContentType = $"image/{Extension.Substring(1)}";
                    break;
                case ".ashx":
                    Header = $"101 Switching Protocols\nUpgrade: websocket\nConnection: Upgrade" +
                        $"\nSec-WebSocket-Accept: " +
                        $"\nSec-WebSocket-Protocol: chat";
                    ContentType = "";
                    break;
                default:
                    if (Extension.Length > 1)
                    {
                        ContentType = $"application/{Extension.Substring(1)}";
                    }
                    else
                    {
                        ContentType = "application/unknown";
                    }
                    break;
            }
            ContentType = (string.IsNullOrEmpty(ContentType) ? "" : $"\nContent-type: {ContentType}\nContent-Length: {FS.Length}");
            var SendingString = $"{(string.IsNullOrEmpty(Header) ? "200 OK" : Header)}{ContentType}";
            return  SendingString;
        }
    }
}
