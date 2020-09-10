using ClearServer.Core.UserController;
using System;
using System.IO;
using System.Net;
using ClearServer.Core.Cookies;
using ClearServer.Core.Security;
using ClearServerCore.Core.Database;
using System.Drawing;
using System.Drawing.Imaging;

namespace ClearServer.Core.Requester
{
    internal class ClientBrowserService
    {
        private readonly HttpListenerContext _clientContext;
        private readonly HttpListenerRequest _request;
        private readonly HttpListenerResponse _response;
        private readonly ClientHandler _handler;
        private readonly DatabaseWorker _databaseWorker;
        private readonly WriteController _writeController;
        private readonly RazorController _razorController;
        private readonly PasswordHasher PasswordHasher = new PasswordHasher();
        private string _filePath = "C:/Users/drdre/source/repos/ClearServer/View";
        public ClientBrowserService(HttpListenerContext clientContext, ClientHandler handler)
        {
            this._clientContext = clientContext;
            this._handler = handler;
            _databaseWorker = DatabaseWorker.GetInstance();
            _response = _clientContext.Response;
            _request = _clientContext.Request;
            _writeController = new WriteController(_response);
            _razorController = new RazorController(_handler, _response);
            BrowserLoader();
        }

        void BrowserLoader()
        {
            var blockquotes = new string[] { };
            var block = false;
            foreach (var extension in blockquotes)
            {
                if (_request.RawUrl.Contains(extension))
                {
                    block = true;
                    break;
                }
            }
            if (_clientContext.Request.Url.Port == 80)
            {
                _response.Redirect("https://itinder.online/");
                _response.Close();
            }
            switch (_clientContext.Request.HttpMethod)
            {
                case "POST":
                    switch (_request.RawUrl)
                    {
                        case "/Auth.php":
                            var formsValues = DataConverter.DataDeserialize(_handler.Message);
                            var authUser = new User()
                            {
                                login = formsValues["login"].ToString(),
                                password = PasswordHasher.PasswordHash(formsValues["password"].ToString())
                            };
                            if ((authUser = _databaseWorker.UserAuth(authUser)) != null)
                            {

                                var authCookie = UserCookies.AuthCookie(authUser.login, authUser.password);
                                authUser.cookie = authCookie.Value;
                                _databaseWorker.UserUpdate(authUser);
                                _response.Cookies.Add(authCookie);
                                _response.StatusCode = (int)HttpStatusCode.MovedPermanently;
                                _response.RedirectLocation = $"/@{authUser.login}";
                                _response.Close();
                            }
                            else
                            {
                                _response.Redirect("/");
                                _response.Close();
                            }
                            break;
                        case "/Register.php":
                            var formValues = DataConverter.DataDeserialize(_handler.Message);
                            var regUser = new User()
                            {
                                login = formValues["regLogin"].ToString(),
                                password = PasswordHasher.PasswordHash(formValues["regPass"].ToString()),
                                name = formValues["name"].ToString().Replace('+', ' ')
                            };
                            if (_databaseWorker.LoginValidate(regUser.login))
                            {
                                var regCookie = UserCookies.AuthCookie(regUser.login, regUser.password);
                                regUser.cookie = regCookie.Value;
                                _databaseWorker.UserRegister(regUser);
                                _response.Cookies.Add(regCookie);
                                _response.StatusCode = (int)HttpStatusCode.MovedPermanently;
                                _response.RedirectLocation = $"/@{regUser.login}";
                                _response.Close();
                            }
                            break;
                        case "/imgLoad.php":
                            Console.WriteLine("ImgLoading");
                            Image img = Image.FromStream(_clientContext.Request.InputStream);
                            img.Save("D:/testssss.png", ImageFormat.Png);
                            _response.Redirect("/bages.html");
                            _response.Close();
                            break;
                    }
                    break;
                case "GET":
                    switch (_request.RawUrl)
                    {
                        case "/":
                            _filePath += (!_handler.IsAuth) ? "/loginForm.html" : "/mainPage.html";
                            _writeController.DefaultWriter(_filePath);
                            break;
                        case { } a when a.Contains("/@"):
                            var profile = _databaseWorker.FindUser(a.Substring(2));
                            _razorController.ProfileLoader(profile, _handler.CurrentUser, _filePath);
                            break;
                        case "/Chat":
                            _filePath += (_handler.IsAuth) ? "/chat.html" : "/loginForm.html";
                            _writeController.DefaultWriter(_filePath);
                            break;
                        case "/Chat2":
                            _filePath += "/chat2.html";
                            _writeController.DefaultWriter(_filePath);
                            break;
                        
                        default:
                            if (!File.Exists(_filePath + _request.RawUrl) | block)
                            {
                                _razorController.ErrorLoader(404);

                            }
                            else if (Path.HasExtension(_request.RawUrl) && File.Exists(_filePath + _request.RawUrl))
                            {
                                _writeController.DefaultWriter(_filePath + _request.RawUrl);
                            }
                            break;
                    }
                    break;
            }
        }

    }
}