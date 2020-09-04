using ClearServer.Core.UserController;
using System;
using System.IO;
using System.Net;

namespace ClearServer.Core.Requester
{
    internal class ClientBrowserService
    {
        private HttpListenerContext ClientContext;
        private HttpListenerRequest Request;
        private HttpListenerResponse Response;
        private ClientHandler Handler;
        private DatabaseWorker DatabaseWorker;
        private WriteController WriteController;
        private RazorController RazorController;
        private string FilePath = "C:/Users/drdre/source/repos/ClearServer/View";

        User CurrentUser
        {
            get
            {
                if (Request.Cookies.Count > 0)
                {
                    User user = null;
                    foreach (Cookie item in Request.Cookies)
                    {
                        user = DatabaseWorker.CookieValidate(item.Value);
                        Handler.isAuth = (user != null);
                        
                    }
                    return user;
                }
                else return null;
            }
        }
        User UserProfile
        {
            get
            {
                if (Request.RawUrl.Contains("@"))
                {
                    return DatabaseWorker.FindUser(Request.RawUrl.Substring(2));
                }
                else return null;
            }
        }
        public ClientBrowserService(HttpListenerContext clientContext, ClientHandler handler)
        {
            this.ClientContext = clientContext;
            this.Handler = handler;
            Response = ClientContext.Response;
            Request = ClientContext.Request;
            WriteController = new WriteController(Response);
            RazorController = new RazorController(Handler, Response);
            DatabaseWorker = new DatabaseWorker();
            BrowserLoader();
        }

        void BrowserLoader()
        {
            string[] blockextension = new string[] { "cshtml", "html", "htm" };
            bool block = false;
            foreach (var extension in blockextension)
            {
                if (Request.RawUrl.Contains(extension))
                {
                    block = true;
                    break;
                }
            }
            switch (ClientContext.Request.HttpMethod)
            {
                case "POST":
                    break;
                case "GET":
                    switch (Request.RawUrl)
                    {
                        case "/":
                            FilePath += (CurrentUser == null) ? "/loginForm.html" : "/mainPage.html";
                            WriteController.DefaultWriter(FilePath);
                            break;
                        case string a when a.Contains("/@"):
                            Console.WriteLine("Profilerequest");
                            RazorController.ProfileLoader(UserProfile, CurrentUser, FilePath);
                            break;
                        default:
                            if (!File.Exists(FilePath + Request.RawUrl) | block)
                            {
                                RazorController.ErrorLoader(404);

                            }
                            else if (Path.HasExtension(Request.RawUrl) && File.Exists(FilePath + Request.RawUrl))
                            {
                                WriteController.DefaultWriter(FilePath + Request.RawUrl);
                            }
                            break;
                    }
                    break;
            }
        }

    }
}