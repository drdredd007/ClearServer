using ClearServer.Core.Requester;
using System;
using System.IO;
using System.Net.Security;
using System.Text;

namespace ClearServer.Core.UserController
{
    internal class NonAuthUserController
    {
        private readonly SslStream ClientStream;
        private readonly RequestContext ReqContext;
        private readonly string ViewPath = "C:/Users/drdre/source/repos/ClearServer/View";

        public NonAuthUserController(SslStream clientStream, RequestContext context)
        {
            this.ClientStream = clientStream;
            this.ReqContext = context;
            ResourceLoad();
        }

        void ResourceLoad()
        {
            string FilePath = "";
            string Header = "";
            switch (ReqContext.RequestMethod)
            { case "GET":
                    switch (ReqContext.RequestUrl)
                    {
                        case "/":
                            FilePath = ViewPath + "/loginForm.html";
                            Header = $"HTTP/1.1 200 OK\nContent-Type: text/html";
                            break;
                        default:
                            if (Path.HasExtension(ReqContext.RequestUrl) && File.Exists(ViewPath+ReqContext.RequestUrl))
                            {
                                Header = ContentType(ReqContext.RequestUrl);
                                FilePath = ViewPath + ReqContext.RequestUrl;
                            }
                            else
                            {
                                Header = $"HTTP/1.1 404 Not Found\n\n";
                                byte[] error = Encoding.UTF8.GetBytes(Header);
                                ClientStream.BeginWrite(error, 0, error.Length, OnClientSend, ClientStream);
                            }
                            break;
                    }

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
                    break;
            }

        }

        private void OnFileRead(IAsyncResult ar)
        {
            if (ar.IsCompleted)
            {
                var file = (byte[])ar.AsyncState;
                ClientStream.BeginWrite(file, 0, file.Length, OnClientSend, null);
            }
        }

        private void OnClientSend(IAsyncResult ar)
        {
            if (ar.IsCompleted)
            {
                ClientStream.Close();
            }
        }

        string ContentType(string Uri)
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
        
    }
}