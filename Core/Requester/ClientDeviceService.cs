using System.Net;

namespace ClearServer.Core.Requester
{
    internal class ClientDeviceService
    {
        private HttpListenerContext clientContext;
        private ClientHandler handler;

        public ClientDeviceService(HttpListenerContext clientContext, ClientHandler handler)
        {
            this.clientContext = clientContext;
            this.handler = handler;
        }
    }
}