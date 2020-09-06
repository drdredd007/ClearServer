using System;
using System.IO;
using System.Net;

namespace ClearServer.Core.UserController
{
    public class WriteController
    {
        HttpListenerResponse _response;
        byte[] _buffer = null;
        public WriteController(HttpListenerResponse response)
        {
            this._response = response;
        }

        public void DefaultWriter(string FilePath)
        {
            FileStream fileStream;
            try
            {
                fileStream = new FileStream(FilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                _response.ContentLength64 = fileStream.Length;
                _response.ContentType = ContentType(FilePath);
                _response.StatusCode = 200;
                _buffer = new byte[fileStream.Length];
                fileStream.BeginRead(_buffer, 0, _buffer.Length, OnFileRead, null);
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
                _response.OutputStream.BeginWrite(_buffer, 0, _buffer.Length, OnClientSend, null);
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
