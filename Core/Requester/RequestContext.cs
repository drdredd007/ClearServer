using ReServer.Core.Classes;
using ReServer.Core.Inrefaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace ClearServer.Core.Requester
{
   public class RequestContext 
    {
        public string message = "";
        private readonly byte[] buffer = new byte[1024];
        public string RequestMethod;
        public string RequestUrl;
        public List<RequestValues> HeadersValues;
        public List<RequestValues> FormValues;
        private TcpClient TcpClient;

        private event Action<SslStream, RequestContext> OnRead = RequestHandler.OnHandle;

        public RequestContext(SslStream ClientStream, TcpClient Client)
        {
            this.TcpClient = Client;
            try
            {
                ClientStream.BeginRead(buffer, 0, buffer.Length, ClientRead, ClientStream);
            }
            catch { return; }
        }
        private void ClientRead(IAsyncResult ar)
        {
            SslStream ClientStream = (SslStream)ar.AsyncState;
            
            if (ar.IsCompleted)
            {
                message = Encoding.UTF8.GetString(buffer);
                Console.WriteLine($"\n{DateTime.Now:g} Client IP:{TcpClient.Client.RemoteEndPoint}\n{message}");
                RequestParse(message);
                HeadersValues = HeaderValues(message);
                FormValues = ContentValues(message);
                OnRead?.Invoke(ClientStream, this);
            }
        }

        private void RequestParse(string Message)
        {
            Match methodParse = Regex.Match(Message, @"(^\w+)\s+([^\s\?]+)[^\s]*\s+HTTP/.*|");
            RequestMethod = methodParse.Groups[1].Value.Trim();
            RequestUrl = methodParse.Groups[2].Value.Trim();
        }
        private List<RequestValues> HeaderValues(string Message)
        {
            var values = new List<RequestValues>();
            var parse = Regex.Matches(Message, @"(.*?): (.*?)\n");
            foreach (Match match in parse)
            {
                values.Add(new RequestValues()
                {
                    Name = match.Groups[1].Value.Trim(),
                    Value = match.Groups[2].Value.Trim()
                });
            }
            return values;
        }
        private List<RequestValues> ContentValues(string Message)
        {
            var values = new List<RequestValues>();
            var output = Message.Trim('\n').Split().Last();
            var parse = Regex.Matches(output, @"([^&].*?)=([^&]*)");
            foreach (Match match in parse)
            {
                values.Add(new RequestValues()
                {
                    Name = match.Groups[1].Value.Trim(),
                    Value = match.Groups[2].Value.Trim()
                });
            }
            return values;
        }
    }
}
