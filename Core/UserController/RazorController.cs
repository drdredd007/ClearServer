using ClearServer.Core.Requester;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Reflection;
using System.Security.Policy;
using System.Text;

namespace ClearServer.Core.UserController
{
    internal class RazorController
    {
        private RequestContext Context;
        private SslStream ClientStream;
        dynamic PageContent;


        public RazorController(RequestContext context, SslStream clientStream)
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
                        PageContent = new { isAuth = true, Name = Context.CurrentUser.name, Login = Context.CurrentUser.login, Skills = Context.CurrentUser.skills };
                        ClientSend(Filepath, Context.CurrentUser.login);
                    }
                    catch (Exception e) { Console.WriteLine(e); }

                }
                else
                {
                    try
                    {
                        PageContent = new { isAuth = false, Name = Context.RequestProfile.name, Login = Context.RequestProfile.login, Skills = Context.RequestProfile.skills };
                        ClientSend(Filepath, "PublicProfile:"+ Context.RequestProfile.login);
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
                PageContent = new { ErrorCode = Code, Message = ((HttpStatusCode)Code).ToString() };
                string ErrorPage = "C:/Users/drdre/source/repos/ClearServer/View/Errors/ErrorPage.cshtml";
                ClientSend(ErrorPage, Code.ToString());
            }
            catch { }

        }

        private void ClientSend(string FilePath, string Key)
        {
            var template = File.ReadAllText(FilePath);
            var result = Engine.Razor.RunCompile(template, Key, null, (object)PageContent);
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