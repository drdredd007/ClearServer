using ReServer.Core.Classes;
using System.Collections.Generic;
using System.Net.Security;

namespace ReServer.Core.Inrefaces
{
    interface IRequestContext
    {
        abstract void GetContext(SslStream Client);
    }
}