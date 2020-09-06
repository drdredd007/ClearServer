using System.Net;

namespace ClearServer.Core.Requester
{
    internal class ClientDeviceService
    {
        private HttpListenerContext _clientContext;
        private ClientHandler _handler;

        public ClientDeviceService(HttpListenerContext clientContext, ClientHandler handler)
        {
            this._clientContext = clientContext;
            this._handler = handler;
        }
    }
}