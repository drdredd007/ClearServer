using ClearServer.Core.Requester;
using ClearServer.Core.Templating;
using System;
using System.IO;
using System.Net;
using System.Net.Security;

namespace ClearServer.Core.UserController
{
    internal class TemplateController
    {
        private RequestContext Context;
        private SslStream ClientStream;

        public TemplateController(RequestContext context, SslStream clientStream)
        {
            this.Context = context;
            this.ClientStream = clientStream;
        }

        public void ProfileLoader(string ViewPath)
        {
            string Filepath = ViewPath + "/profile.cshtml";
            if (Context.RequestProfile != null)
            {
                if (Context.CurrentUser != null && Context.RequestProfile.login == Context.CurrentUser.login)
                {
                    try
                    {
                        var model = new { isAuth = true, Name = Context.CurrentUser.name, Login = Context.CurrentUser.login, Skills = Context.CurrentUser.skills };
                        ClientSend(Filepath, model);
                    }
                    catch (Exception e) { Console.WriteLine(e); }

                }
                else
                {
                    try
                    {
                        var model = new { isAuth = false, Name = Context.RequestProfile.name, Login = Context.RequestProfile.login, Skills = Context.RequestProfile.skills };
                        ClientSend(Filepath, model);
                    }
                    catch (Exception e) { Console.WriteLine(e); }
                }
            }
            else
            {
                ErrorLoader(404);
            }


        }

        public void ErrorLoader(int Code)
        {
            try
            {
                var model = new { ErrorCode = Code, Message = ((HttpStatusCode)Code).ToString() };
                string ErrorPage = "C:/Users/drdre/source/repos/ClearServer/View/Errors/ErrorPage.cshtml";
                ClientSend(ErrorPage, model);
            }
            catch { }

        }

        private void ClientSend(string FilePath, object model)
        {
            string template = File.ReadAllText(FilePath);
            string result = TemplateEngine.Render(template, model);
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(result);
            ClientStream.BeginWrite(buffer, 0, buffer.Length, OnClientSend, ClientStream);
        }

        private void OnClientSend(IAsyncResult ar)
        {
            if (ar.IsCompleted)
            {
                ClientStream.Close();
            }
        }
    }
}
