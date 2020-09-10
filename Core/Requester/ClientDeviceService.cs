using ClearServer.Core.Cookies;
using ClearServer.Core.Security;
using ClearServerCore.Core.Database;
using ClearServerCore.Core.UserController.DeviceController;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Text;

namespace ClearServer.Core.Requester
{
    internal class ClientDeviceService
    {
        private HttpListenerContext _clientContext;
        private ClientHandler _handler;
        private PasswordHasher Hasher;
        private DatabaseWorker databaseWorker;

        public ClientDeviceService(HttpListenerContext clientContext, ClientHandler handler)
        {
            this._clientContext = clientContext;
            this._handler = handler;
            databaseWorker = DatabaseWorker.GetInstance();
            Hasher = new PasswordHasher();
            DeviceHandler();
        }

        private void DeviceHandler()
        {
            switch (_clientContext.Request.RawUrl)
            {
                case "/Device/LoginForm":
                    var loginform = DataConverter.DataDeserialize(_handler.Message);
                    User authUser = new User()
                    {
                        login = loginform["Login"] != null ? loginform["Login"].ToString() : "null",
                        password = loginform["Password"] != null ? Hasher.PasswordHash(loginform["Password"].ToString()) : "null"
                    };

                    var userKey = UserCookies.AuthCookie(authUser.login, authUser.password);

                    authUser = databaseWorker.UserAuth(authUser);
                    if (authUser != null)
                    {
                        authUser.cookie = userKey.Value;
                        _clientContext.Response.AddHeader("UserKey", userKey.Value);
                        _clientContext.Response.AddHeader("Auth", "true");
                        var deviceUser = new DeviceUser()
                        {
                            Name = authUser.name,
                            Login = authUser.login,
                            Skills = authUser.skills
                        };
                        var json = JsonConvert.SerializeObject(deviceUser, Formatting.Indented);
                        var buffer = Encoding.UTF8.GetBytes(json);

                        _clientContext.Response.ContentLength64 = buffer.Length;
                        _clientContext.Response.ContentType = "text/json";
                        _clientContext.Response.OutputStream.BeginWrite(buffer, 0, buffer.Length, OnSend, null);

                        databaseWorker.UserUpdate(authUser);
                    }
                    else
                    {
                        _clientContext.Response.AddHeader("UserKey", "none");
                        _clientContext.Response.AddHeader("Auth", "false");
                    }
                    _clientContext.Response.Close();

                    break;
                case "/Device/LaunchAuth":

                    var key = _clientContext.Request.Headers["UserKey"];
                    if (!string.IsNullOrEmpty(key))
                    {
                        var user = databaseWorker.CookieValidate(key);
                        if (user != null)
                        {
                            _clientContext.Response.AddHeader("Auth", "true");
                        }
                        else
                        {
                            _clientContext.Response.AddHeader("Auth", "false");
                        }
                        
                    }
                    _clientContext.Response.Close();
                    break;
                default:
                    Console.WriteLine(_clientContext.Request.RawUrl);
                    break;
            }
        }

        private void OnSend(IAsyncResult ar)
        {
            if (ar.IsCompleted)
            {
                _clientContext.Response.Close();
            }
        }
    }
}