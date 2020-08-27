using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace ClearServer.Core.UserController
{
   public class WriteController
    {
        SslStream ClientStream;
        public WriteController(SslStream ClientStream)
        {
            this.ClientStream = ClientStream;
        }

        public void DefaultWriter(string Header, string FilePath)
        {
            FileStream fileStream;
            try
            {
                fileStream = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
                Header = $"{Header}\nContent-Length: {fileStream.Length}\n\n";
                ClientStream.Write(Encoding.UTF8.GetBytes(Header));
                byte[] response = new byte[fileStream.Length];
                fileStream.BeginRead(response, 0, response.Length, OnFileRead, response);
            }
            catch { }
        }

        public string ContentType(string Uri)
        {
            string extension = Path.GetExtension(Uri);
            string Header = "HTTP/1.1 200 OK\nContent-Type:";
            switch (extension)
            {
                case ".html":
                case ".htm":
                    return $"{Header} text/html";
                case ".css":
                    return $"{Header} text/css";
                case ".js":
                    return $"{Header} text/javascript";
                case ".jpg":
                case ".jpeg":
                case ".png":
                case ".gif":
                    return $"{Header} image/{extension}";
                default:
                    if (extension.Length > 1)
                    {
                        return $"{Header} application/" + extension.Substring(1);
                    }
                    else
                    {
                        return $"{Header} application/unknown";
                    }
            }
        }

        public void OnFileRead(IAsyncResult ar)
        {
            if (ar.IsCompleted)
            {
                var file = (byte[])ar.AsyncState;
                ClientStream.BeginWrite(file, 0, file.Length, OnClientSend, null);
            }
        }

        public void OnClientSend(IAsyncResult ar)
        {
            if (ar.IsCompleted)
            {
                ClientStream.Close();
            }
        }
    }
}
