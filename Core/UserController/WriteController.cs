using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Text;

namespace ClearServer.Core.UserController
{
    public class WriteController
    {
        HttpListenerResponse Response;
        byte[] buffer = null;
        public WriteController(HttpListenerResponse response)
        {
            this.Response = response;
        }

        public void DefaultWriter(string FilePath)
        {
            FileStream fileStream;
            try
            {
                fileStream = new FileStream(FilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                Response.ContentLength64 = fileStream.Length;
                Response.ContentType = ContentType(FilePath);
                Response.StatusCode = 200;
                buffer = new byte[fileStream.Length];
                fileStream.BeginRead(buffer, 0, buffer.Length, OnFileRead, null);
            }
            catch { }
        }

        public string ContentType(string path)
        {
            string extension = Path.GetExtension(path);
            switch (extension)
            {
                case ".html":
                case ".htm":
                    return $"text/html";
                case ".css":
                    return $"text/css";
                case ".js":
                    return $"text/javascript";
                case ".jpg":
                case ".jpeg":
                case ".png":
                case ".gif":
                    return $"image/{extension}";
                default:
                    if (extension.Length > 1)
                    {
                        return $"application/" + extension.Substring(1);
                    }
                    else
                    {
                        return $"application/unknown";
                    }
            }
        }

        public void OnFileRead(IAsyncResult ar)
        {
            if (ar.IsCompleted)
            {
                Response.OutputStream.BeginWrite(buffer, 0, buffer.Length, OnClientSend, null);
            }
        }

        public void OnClientSend(IAsyncResult ar)
        {
            if (ar.IsCompleted)
            {
                //Response.Close();
            }
        }
    }
}
