using System;
using System.IO;
using System.Net;
using System.Text;
using React;

namespace ClearServerCore.Core.UserController
{
    public class WriteController
    {
        HttpListenerResponse _response;
        FileStream FS;
        byte[] _buffer = null;
        public WriteController(HttpListenerResponse response)
        {
            this._response = response;
        }

        public void DefaultWriter(string FilePath)
        {
            try
            {
                FS = new FileStream(FilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                _response.ContentLength64 = FS.Length;
                _response.ContentType = ContentType(FilePath);
                _response.StatusCode = 200;
                _buffer = new byte[FS.Length];
                FS.BeginRead(_buffer, 0, _buffer.Length, OnFileRead, null);
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

        public async void OnClientSend(IAsyncResult ar)
        {
            if (ar.IsCompleted)
            {
                await FS.DisposeAsync();
                _response.Close();
            }
        }

        internal void BabelTest(string filePath)
        {
            Console.WriteLine("Start");
            try
            {
                var test = ReactEnvironment.GetCurrentOrThrow.Babel;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            //var babel = ReactEnvironment.Current.Babel;
            //var result = babel.TransformFile(filePath);
            //Console.WriteLine(result);
            //var output = Encoding.UTF8.GetBytes(result);
            //_response.StatusCode = 200;
            //_response.OutputStream.BeginWrite(output, 0, output.Length, null, null);
        }


        public class Cup
        {
            private bool isEmpty { get; set; }
            private float volume { get; set; }
            private object content { get; set; }

        }
    }
}
