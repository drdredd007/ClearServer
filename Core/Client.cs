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

            string Message = "";
            byte[] Buffer = new byte[1024];
            int Count;
            while ((Count = Client.GetStream().Read(Buffer, 0, Buffer.Length)) > 0)
            {
                Message += Encoding.UTF8.GetString(Buffer, 0, Count);

                if (Message.IndexOf("\r\n\r\n") >= 0 || Message.Length > 4096)
                {
                    Console.WriteLine(Message);
                    break;
                }
            }

            Match ReqMatch = Regex.Match(Message, @"^\w+\s+([^\s\?]+)[^\s]*\s+HTTP/.*|");
            if (ReqMatch == Match.Empty)
            {
                ErrorWorker.SendError(Client, 400);
                return;
            }
            string RequestUri = ReqMatch.Groups[1].Value;
            RequestUri = Uri.UnescapeDataString(RequestUri);
            if (RequestUri.IndexOf("..") >= 0)
            {
                ErrorWorker.SendError(Client, 400);
                return;
            }
            if (RequestUri.EndsWith("/"))
            {
                RequestUri += "index.html";
            }

            string FilePath = $"D:/Web/DreamWeaver_proj{RequestUri}";

            if (!File.Exists(FilePath))
            {
                ErrorWorker.SendError(Client, 404);
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
            try
            {
                FS = new FileStream(FilePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            catch (Exception)
            {
                ErrorWorker.SendError(Client, 500);
                return;
            }

            string Headers = $"HTTP/1.1 200 OK\nContent-Type: {ContentType}\nContent-Length: {FS.Length}\n\n";
            byte[] HeadersBuffer = Encoding.ASCII.GetBytes(Headers);
            Client.GetStream().Write(HeadersBuffer, 0, HeadersBuffer.Length);

            while (FS.Position < FS.Length)
            {
                Count = FS.Read(Buffer, 0, Buffer.Length);
                Client.GetStream().Write(Buffer, 0, Count);
            }

            FS.Close();
            Client.Close();
        }
    }
}
