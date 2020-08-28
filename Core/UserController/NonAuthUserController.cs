using ClearServer.Core.Requester;
using System;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Net.Security;
using System.Text;

namespace ClearServer.Core.UserController
{
    internal class NonAuthUserController
    {
        private readonly SslStream ClientStream;
        private readonly RequestContext Context;
        private readonly WriteController WriteController;
        private readonly AuthorizationController AuthorizationController;

        private readonly string ViewPath = "C:/Users/drdre/source/repos/ClearServer/View";

        public NonAuthUserController(SslStream clientStream, RequestContext context)
        {
            this.ClientStream = clientStream;
            this.Context = context;
            this.WriteController = new WriteController(clientStream);
            this.AuthorizationController = new AuthorizationController(clientStream, context);
            ResourceLoad();
        }

        void ResourceLoad()
        {
            string[] blockextension = new string[] {"cshtml", "html", "htm"};
            bool block = false;
            foreach (var item in blockextension)
            {
                if (Context.RequestUrl.Contains(item))
                {
                    block = true;
                    break;
                }
            }
            string FilePath = "";
            string Header = "";
            var RazorController = new RazorController(Context, ClientStream);
            
            switch (Context.RequestMethod)
            {
                case "GET":
                    switch (Context.RequestUrl)
                    {
                        case "/":
                            FilePath = ViewPath + "/loginForm.html";
                            Header = $"HTTP/1.1 200 OK\nContent-Type: text/html";
                            WriteController.DefaultWriter(Header, FilePath);
                            break;
                        case "/profile":
                            RazorController.ProfileLoader(ViewPath);
                            break;
                        default:
                            if (!File.Exists(ViewPath + Context.RequestUrl) | block)
                            {
                                RazorController.ErrorLoader(404);
                               
                            }                            
                            else if (Path.HasExtension(Context.RequestUrl) && File.Exists(ViewPath + Context.RequestUrl))
                            {
                                Header = WriteController.ContentType(Context.RequestUrl);
                                FilePath = ViewPath + Context.RequestUrl;
                                WriteController.DefaultWriter(Header, FilePath);
                            }                            
                            break;
                    }
                    break;

                case "POST":
                    AuthorizationController.MethodRecognizer();
                    break;

            }

        }

    }
}