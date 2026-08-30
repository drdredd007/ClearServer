using ClearServer.Core.Cookies;
using ClearServer.Core.Requester;
using ClearServer.Core.Security;
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
        TemplateController TemplateController;
        PasswordHasher PasswordHasher;
        public AuthorizationController(SslStream clientStream, RequestContext context)
        {
            ClientStream = clientStream;
            Context = context;
            DatabaseWorker = new DatabaseWorker();
            WriteController = new WriteController(ClientStream);
            TemplateController = new TemplateController(context, clientStream);
            PasswordHasher = new PasswordHasher();
        }

        internal void MethodRecognizer()
        {
            if (Context.FormValues.Count == 2 && Context.FormValues.Any(x => x.Name == "password")) Authorize();
            else if (Context.FormValues.Count == 3 && Context.FormValues.Any(x => x.Name == "regPass")) Registration();
            else
            {
                TemplateController.ErrorLoader(401);
            }
        }

        private void Authorize()
        {
            var values = Context.FormValues;
            string login = values[0].Value;
            string password = values[1].Value;

            var user = DatabaseWorker.UserAuth(login);
            if (user != null && PasswordHasher.Verify(password, user.password))
            {
                cookies = new UserCookies();
                user.cookie = cookies.AuthCookie;
                DatabaseWorker.UserUpdate(user);
                var response = Encoding.UTF8.GetBytes($"HTTP/1.1 301 Moved Permanently\nLocation: /@{user.login}\nSet-Cookie: {cookies.AuthCookie}; Expires={DateTime.Now.AddDays(2):R}; Secure; HttpOnly\n\n");
                ClientStream.BeginWrite(response, 0, response.Length, WriteController.OnClientSend, null);


            }
            else
            {
                TemplateController.ErrorLoader(401);

            }
        }

        private void Registration()
        {
            var values = Context.FormValues;
            var user = new User()
            {
                name = values[0].Value,
                login = values[1].Value,
                password = PasswordHasher.PasswordHash(values[2].Value),
            };
            cookies = new UserCookies();
            user.cookie = cookies.AuthCookie;
            if (DatabaseWorker.LoginValidate(user.login))
            {
                Console.WriteLine("User ready");
                Console.WriteLine($"{user.password} {user.password.Trim().Length}");
                DatabaseWorker.UserRegister(user);
                var response = Encoding.UTF8.GetBytes($"HTTP/1.1 301 Moved Permanently\nLocation: /@{user.login}\nSet-Cookie: {user.cookie}; Expires={DateTime.Now.AddDays(2):R}; Secure; HttpOnly\n\n");
                ClientStream.BeginWrite(response, 0, response.Length, WriteController.OnClientSend, null);
            }
            else
            {
                TemplateController.ErrorLoader(401);
            }
        }
    }
}