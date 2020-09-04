using ClearServer.Core.Requester;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.IO;
using System.Net;

namespace ClearServer.Core.UserController
{
    internal class RazorController
    {
        private ClientHandler Context;
        private HttpListenerResponse ClientResponse;
        dynamic PageContent;


        public RazorController(ClientHandler context, HttpListenerResponse clientResponse)
        {
            this.Context = context;
            this.ClientResponse = clientResponse;

        }

        public void ProfileLoader(User Profile, User CurrentUser, string Filepath)
        {
            Filepath += "/profile.cshtml";
            if (Profile != null)
            {
                if (Context.isAuth)
                {
                    try
                    {
                        PageContent = new { isAuth = true, Name = CurrentUser.name, Login = CurrentUser.login, Skills = CurrentUser.skills };
                        ClientSend(Filepath, CurrentUser.login);
                    }
                    catch (Exception e) { Console.WriteLine(e); }

                }
                else
                {
                    try
                    {
                        PageContent = new { isAuth = false, Name = Profile.name, Login = Profile.login, Skills = Profile.skills };
                        ClientSend(Filepath, "PublicProfile:" + Profile.login);
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
                ClientResponse.StatusCode = Code;
                ClientSend(ErrorPage, Code.ToString());
            }
            catch { }

        }

        private void ClientSend(string FilePath, string Key)
        {
            var template = File.ReadAllText(FilePath);
            var result = Engine.Razor.RunCompile(template, Key, null, (object)PageContent);
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(result);
            ClientResponse.ContentLength64 = buffer.Length;
            ClientResponse.OutputStream.BeginWrite(buffer, 0, buffer.Length, OnClientSend, null);
        }

        private void OnClientSend(IAsyncResult ar)
        {
            if (ar.IsCompleted)
            {
                ClientResponse.Close();
            }
        }
    }
}