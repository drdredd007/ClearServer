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
        public static void OnHandle(SslStream ClientStream, RequestContext context)
        {

            if (context.CurrentUser != null)
            {
                new AuthUserController(ClientStream, context);
            }
            else 
            {
                new NonAuthUserController(ClientStream, context);
            };
        }
    }
}
