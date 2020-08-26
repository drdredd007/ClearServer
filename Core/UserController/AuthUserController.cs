using ClearServer.Core.Requester;
using System.Net.Security;

namespace ClearServer.Core.UserController
{
    internal class AuthUserController
    {
        private readonly SslStream clientStream;
        private readonly RequestContext context;

        public AuthUserController(SslStream clientStream, RequestContext context)
        {
            this.clientStream = clientStream;
            this.context = context;
            clientStream.Close();
        }
    }
}