using ClearServer.Core.UserController;
using ReServer.Core.Classes;
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
        public string Message = "";
        private readonly byte[] buffer = new byte[1024];
        public string RequestMethod;
        public string RequestUrl;
        public User RequestProfile;
        public User CurrentUser = null;
        public List<RequestValues> HeadersValues;
        public List<RequestValues> FormValues;
        private TcpClient TcpClient;

        private event Action<SslStream, RequestContext> OnRead = RequestHandler.OnHandle;

        DatabaseWorker databaseWorker = new DatabaseWorker();

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
                Message = Encoding.UTF8.GetString(buffer);
                Message = Uri.UnescapeDataString(Message);
                Console.WriteLine($"\n{DateTime.Now:g} Client IP:{TcpClient.Client.RemoteEndPoint}\n{Message}");
                RequestParse();
                HeadersValues = HeaderValues();
                FormValues = ContentValues();
                UserParse();
                ProfileParse();
                OnRead?.Invoke(ClientStream, this);
            }
        }

        private void RequestParse()
        {
            Match methodParse = Regex.Match(Message, @"(^\w+)\s+([^\s\?]+)[^\s]*\s+HTTP/.*|");
            RequestMethod = methodParse.Groups[1].Value.Trim();
            RequestUrl = methodParse.Groups[2].Value.Trim();
        }
        private void UserParse()
        {
            string cookie;
            try
            {
                if (HeadersValues.Any(x => x.Name.Contains("Cookie")))
                {
                    cookie = HeadersValues.FirstOrDefault(x => x.Name.Contains("Cookie")).Value;
                    try
                    {
                        CurrentUser = databaseWorker.CookieValidate(cookie);
                    }
                    catch { }
                }
            }
            catch { }

        }
        private List<RequestValues> HeaderValues()
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

        private void ProfileParse()
        {
            if (RequestUrl.Contains("@"))
            {
                RequestProfile = databaseWorker.FindUser(RequestUrl.Substring(2));
                RequestUrl = "/profile";
            }
        }
        private List<RequestValues> ContentValues()
        {
            var values = new List<RequestValues>();
            var output = Message.Trim('\n').Split().Last();
            var parse = Regex.Matches(output, @"([^&].*?)=([^&]*\b)");
            foreach (Match match in parse)
            {
                values.Add(new RequestValues()
                {
                    Name = match.Groups[1].Value.Trim(),
                    Value = match.Groups[2].Value.Trim().Replace('+', ' ')
                });
            }
            return values;
        }
    }
}
