using System.Net;

namespace ClearServer.Core.Requester
{
    public class RequestHandler
    {
        public static void OnHandle(HttpListenerContext ClientContext, ClientHandler handler)
        {
            if (handler.IsMobile)
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
