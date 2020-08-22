using System;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;

namespace ClearServer
{
    public static class ErrorWorker
    {
        public static void SendError(SslStream ClientStream, int Code)
        {
            string CodeStr = $"{Code} {(HttpStatusCode)Code}";
            string Html = $"<html><body><h1>{CodeStr}</h1></body></html>";
            string Str = $"HTTP/1.1 {CodeStr}\nContent-type: text/html\nContent-Length:{Html.Length}\n\n{Html}";
            byte[] Buffer = Encoding.ASCII.GetBytes(Str);
            try
            {
                ClientStream.Write(Buffer, 0, Buffer.Length);
                ClientStream.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}