using System;
using System.IO;
using System.Net;
using ClearServer.Core.Requester;
using ClearServerCore.Core.Database;
using ClearServerCore.Core.RazorController;
using ClearServerCore.Core.Utils;
using RazorLight;
namespace ClearServerCore.Core.UserController
{
    internal class RazorController
    {
        private ClientHandler handler;
        private HttpListenerResponse _clientResponse;
        private UserModel userpage;
        private RazorLightEngine Engine;

        public RazorController(ClientHandler handler, HttpListenerResponse clientResponse)
        {
            this.handler = handler;
            this._clientResponse = clientResponse;
            Engine = RazorEngine.Engine;

        }

        public void ProfileLoader(User Profile, User CurrentUser, string Filepath)
        {
            Filepath += "/profile.cshtml";
            if (Profile != null)
            {
                if (Profile == CurrentUser)
                {
                    try
                    {
                        userpage = new UserModel()
                        {
                            isAuth = true,
                            user = CurrentUser
                        };
                        ClientSend(Filepath, userpage, CurrentUser.name + CurrentUser.login);
                    }
                    catch (Exception e) { Console.WriteLine(e); }

                }
                else
                {
                    try
                    {
                        userpage = new UserModel()
                        {
                            isAuth = false,
                            user = Profile
                        };
                        ClientSend(Filepath, userpage, "PublicProfile:" + Profile.name);
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
                ErrorModel errorModel = new ErrorModel()
                {
                    Code = Code
                };
                string errorPage = "C:/Users/drdre/source/repos/ClearServer/View/Errors/ErrorPage.cshtml";
                _clientResponse.StatusCode = Code;
                ClientSend(errorPage, errorModel, Code.ToString());
            }
            catch { }

        }

        private void ClientSend(string FilePath, object model, string Key)
        {
            var template = File.ReadAllText(FilePath);
            var result = Engine.CompileRenderStringAsync(Key, template, model);
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(result.Result);
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

        internal void MainPage(string filePath, HttpListenerContext context)
        {
            var RequestModel = new Request()
            {
                context = context,
                user = handler.CurrentUser
            };
            ClientSend(filePath, RequestModel, "Main page" + handler.CurrentUser.cookie);
        }
    }
}