using ClearServer.Core.Parser;
using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace ClearServer
{
    class Client
    {

        public Client(TcpClient Client)
        {
            var ClientStream = Client.GetStream();
            DatabaseWorker databaseWorker = new DatabaseWorker();
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

                        break;
                    }

                }
            }
            catch (Exception)
            {

            }

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
                ClientStream.Close(100);
            }
        }
        public static void Response(NetworkStream ClientStream, string Message)
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

            string FilePath = $"D:/Web/DreamWeaver_proj{RequestUri}";

            if (!File.Exists(FilePath))
            {
                ErrorWorker.SendError(ClientStream, 404);
                return;
            }

            string Extension = RequestUri.Substring(RequestUri.LastIndexOf('.'));

            string ContentType = "";
            switch (Extension)
            {
                case ".htm":
                case ".html":
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

            FileStream FS;
            FS = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            string Headers = $"HTTP/1.1 200 OK\nContent-Type: {ContentType}\nContent-Length: {FS.Length}\n\n";
            byte[] HeadersBuffer = Encoding.ASCII.GetBytes(Headers);
            try
            {
                int Count;
                byte[] repsBuffer = new byte[FS.Length];
                ClientStream.Write(HeadersBuffer, 0, HeadersBuffer.Length);
                while (FS.Position < FS.Length)
                {
                    Count = FS.Read(repsBuffer, 0, repsBuffer.Length);
                    ClientStream.Write(repsBuffer, 0, Count);
                }
            }
            catch (Exception)
            {

            }

            FS.Close();

        }
    }
}
