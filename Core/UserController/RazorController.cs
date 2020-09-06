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
        private ClientHandler _context;
        private HttpListenerResponse _clientResponse;
        dynamic _pageContent;


        public RazorController(ClientHandler context, HttpListenerResponse clientResponse)
        {
            this._context = context;
            this._clientResponse = clientResponse;

        }

        public void ProfileLoader(User Profile, User CurrentUser, string Filepath)
        {
            Filepath += "/profile.cshtml";
            if (Profile != null)
            {
                if (_context.IsAuth)
                {
                    try
                    {
                        _pageContent = new { isAuth = true, Name = CurrentUser.name, Login = CurrentUser.login, Skills = CurrentUser.skills };
                        ClientSend(Filepath, CurrentUser.login);
                    }
                    catch (Exception e) { Console.WriteLine(e); }

                }
                else
                {
                    try
                    {
                        _pageContent = new { isAuth = false, Name = Profile.name, Login = Profile.login, Skills = Profile.skills };
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
                _pageContent = new { ErrorCode = Code, Message = ((HttpStatusCode)Code).ToString() };
                string errorPage = "C:/Users/drdre/source/repos/ClearServer/View/Errors/ErrorPage.cshtml";
                _clientResponse.StatusCode = Code;
                ClientSend(errorPage, Code.ToString());
            }
            catch { }

        }

        private void ClientSend(string FilePath, string Key)
        {
            var template = File.ReadAllText(FilePath);
            var result = Engine.Razor.RunCompile(template, Key, null, (object)_pageContent);
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(result);
            _clientResponse.ContentLength64 = buffer.Length;
            _clientResponse.OutputStream.BeginWrite(buffer, 0, buffer.Length, OnClientSend, null);
        }

        private void OnClientSend(IAsyncResult ar)
        {
            if (ar.IsCompleted)
            {
                _clientResponse.Close();
            }
        }
    }
}