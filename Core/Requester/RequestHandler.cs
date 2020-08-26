using ClearServer.Core.Cookies;
using ClearServer.Core.UserController;
using ReServer.Core.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
namespace ClearServer.Core.Requester
{
    public class RequestHandler
    {
        static UserCookies userCookies = new UserCookies("TestLogin","TestPassword");
        static DatabaseWorker databaseWorker = new DatabaseWorker();
        static User CurrentUser;
        public static void OnHandle(SslStream ClientStream, RequestContext context)
        {
            RequestValues cookie = null;
            try { cookie = context.HeadersValues.Single(x => x.Name.Contains("Cookie")); }
            catch { }

            if ((cookie != null) && (CurrentUser = databaseWorker.CookieValidate(cookie.Value)) != null)
            {
                new AuthUserController(ClientStream, context);
                //Console.WriteLine("User auth");
                //byte[] response = Encoding.UTF8.GetBytes($"HTTP/1.1 200 OK\nContent-Type: text/html\n\n<html><h1>Hello {CurrentUser.login}</h1></html>");
                //ClientStream.BeginWrite(response, 0, response.Length, ClientWrite, ClientStream);
            }

            else 
            {
                new NonAuthUserController(ClientStream, context);
                //byte[] response = Encoding.UTF8.GetBytes($"HTTP/1.1 200 OK\nContent-Type: text/html\nSet-Cookie: {userCookies.AuthCookie}; Expires={DateTime.Now.AddHours(1):R}; Secure; HttpOnly\n\n<html><h1>Hello Stranger</h1></html>");
                //Console.WriteLine(userCookies.AuthCookie);
                //ClientStream.BeginWrite(response,0,response.Length,ClientWrite,ClientStream);
            };
        }

        private static void ClientWrite(IAsyncResult ar)
        {
            if (ar.IsCompleted)
            {
                var client = (SslStream)ar.AsyncState;
                client.Close();
            }

        }
    }
}
