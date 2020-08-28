using ClearServer.Core.Requester;
using System.IO;
using System.Net.Security;

namespace ClearServer.Core.UserController
{
    internal class AuthUserController
    {
        private readonly SslStream ClientStream;
        private readonly RequestContext Context;
        private readonly User CurrentUser;
        private readonly WriteController WriteController;
        private readonly string ViewPath = "C:/Users/drdre/source/repos/ClearServer/View";

        public AuthUserController(SslStream clientStream, RequestContext context)
        {
            this.ClientStream = clientStream;
            this.Context = context;
            this.CurrentUser = context.CurrentUser;
            WriteController = new WriteController(ClientStream);
            ResourceLoader();
        }

        void ResourceLoader()
        {
            string[] blockextension = new string[] { "cshtml", "html", "htm" };
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
                            FilePath = ViewPath + "/test.html";
                            Header = WriteController.ContentType("/mainpage.html");
                            WriteController.DefaultWriter(Header, FilePath);
                            break;
                        case "/profile":
                            FilePath = ViewPath + "/profile.cshtml";
                            RazorController.ProfileLoader(ViewPath);
                            break;
                        default:
                            if (!File.Exists(ViewPath + Context.RequestUrl) || block)
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
                    break;

            }
        }
    }
}