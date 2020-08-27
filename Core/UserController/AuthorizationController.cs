using ClearServer.Core.Cookies;
using ClearServer.Core.Requester;
using System;
using System.Linq;
using System.Net.Security;
using System.Text;

namespace ClearServer.Core.UserController
{
    internal class AuthorizationController
    {
        private SslStream ClientStream;
        private RequestContext Context;
        private UserCookies cookies;
        private WriteController WriteController;
        DatabaseWorker DatabaseWorker;
        RazorController RazorController;

        public AuthorizationController(SslStream clientStream, RequestContext context)
        {
            this.ClientStream = clientStream;
            this.Context = context;
            this.DatabaseWorker = new DatabaseWorker();
            this.WriteController = new WriteController(ClientStream);
            RazorController = new RazorController(context, clientStream);
        }

        internal void MethodRecognizer()
        {
            if (Context.FormValues.Count == 2 && Context.FormValues.Any(x => x.Name == "password")) Authorize();
            else if (Context.FormValues.Count == 3 && Context.FormValues.Any(x => x.Name == "passwordReg")) Registration();
            else
            {
                RazorController.ErrorLoader(401);
            }
        }

        private void Authorize()
        {
            var values = Context.FormValues;
            var user = new User()
            {
                login = values[0].Value,
                password = values[1].Value
            };
            user = DatabaseWorker.UserAuth(user);
            if (user != null)
            {
                cookies = new UserCookies(user.login, user.password);
                user.cookie = cookies.AuthCookie;
                DatabaseWorker.UserUpdate(user);
                var response = Encoding.UTF8.GetBytes($"HTTP/1.1 301 Moved Permanently\nLocation: /profile\nSet-Cookie: {cookies.AuthCookie}; Expires={DateTime.Now.AddDays(2):R}; Secure; HttpOnly\n\n");
                ClientStream.BeginWrite(response, 0, response.Length, WriteController.OnClientSend, null);
                

            }
            else
            {
                RazorController.ErrorLoader(401);

            }
        } 

        private void Registration()
        {
            var values = Context.FormValues;
            cookies = new UserCookies(values[1].Value, values[2].Value);
            var user = new User()
            {
                name = values[0].Value,
                login = values[1].Value,
                password = values[2].Value,
                cookie = cookies.AuthCookie
               
            };
            if (DatabaseWorker.LoginValidate(user.login))
            {
                DatabaseWorker.UserRegister(user);
                var response = Encoding.UTF8.GetBytes($"HTTP/1.1 301 Moved Permanently\nLocation: /profile\nSet-Cookie: {user.cookie}; Expires={DateTime.Now.AddDays(2):R}; Secure; HttpOnly\n\n");
                ClientStream.BeginWrite(response, 0, response.Length, WriteController.OnClientSend, null);
            }
            else
            {
                RazorController.ErrorLoader(401);
            }
        }
    }
}