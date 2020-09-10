using ClearServer.Core.Requester;
using ClearServerCore.Core.Database;
using System;
using System.IO;
using System.Net;
using ClearServerCore.Core.Utils;
using RazorLight;
using ClearServerCore.Core.RazorController;

namespace ClearServer.Core.UserController
{
    internal class RazorController
    {
        private ClientHandler _context;
        private HttpListenerResponse _clientResponse;
        private UserModel userpage;
        private RazorLightEngine Engine;


        public RazorController(ClientHandler context, HttpListenerResponse clientResponse)
        {
            this._context = context;
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
                        ClientSend(Filepath, typeof(UserModel), userpage,CurrentUser.login);
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
                        ClientSend(Filepath,typeof(UserModel), userpage,"PublicProfile:" + Profile.login);
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
                ClientSend(errorPage,typeof(ErrorModel),errorModel,Code.ToString());
            }
            catch { }

        }

        private void ClientSend(string FilePath,Type type, object model, string Key)
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
    }
}