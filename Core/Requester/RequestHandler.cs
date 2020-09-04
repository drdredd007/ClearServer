using ClearServer.Core.UserController;
using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Text;

namespace ClearServer.Core.Requester
{
    public class RequestHandler
    {
        public static void OnHandle(HttpListenerContext ClientContext, ClientHandler handler)
        {
            if (handler.isMobile)
            {
                new ClientDeviceService(ClientContext, handler);
            }
            else
            {
              new ClientBrowserService(ClientContext, handler);
            }
        }
    }
}
