using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HttpMachine;
using IHttpMachine;

namespace ClearServer
{
    class ParserHandler : IHttpParserCombinedDelegate
    {
        private enum Keys
        {
            AuthKey,
            RegisterKeys
        }

        public string[] keysValues = { "nWcX!@q0iD1FqsAAf=l" };

        public string finalMessage, Method;
        public string[] logpass;
        
        public bool HasError { get; set; }
        public MessageType MessageType { get; private set; }
        public void OnResponseType(IHttpCombinedParser combinedParser)
        {
            MessageType = MessageType.Response;
        }
        public void OnRequestType(IHttpCombinedParser combinedParser)
        {
            MessageType = MessageType.Request;
        }

        public void OnMessageBegin(IHttpCombinedParser combinedParser) 
        {
        
        }
        public void OnHeaderName(IHttpCombinedParser combinedParser, string name) 
        {

        }
        public void OnHeaderValue(IHttpCombinedParser combinedParser, string value) 
        {
            //Parse login-password
            if (Method == "POST" && value.Contains(keysValues[(int)Keys.AuthKey]))
            {                
                var values = value.Split('&');
                logpass = values.Where(val => val != keysValues[(int)Keys.AuthKey] && val != null).ToArray();
            }
        }
        public void OnHeadersEnd(IHttpCombinedParser combinedParser) { }
        public void OnMethod(IHttpCombinedParser combinedParser, string method) { Method = method; }
        public void OnRequestUri(IHttpCombinedParser combinedParser, string requestUri) { }
        public void OnPath(IHttpCombinedParser combinedParser, string path) { }
        public void OnFragment(IHttpCombinedParser combinedParser, string fragment) { }
        public void OnQueryString(IHttpCombinedParser combinedParser, string queryString) {}
        public void OnResponseCode(IHttpCombinedParser combinedParser, int statusCode, string statusReason) { }
        public void OnBody(IHttpCombinedParser combinedParser, ArraySegment<byte> data) { }
        public void OnMessageEnd(IHttpCombinedParser combinedParser) { }
        public void OnTransferEncodingChunked(IHttpCombinedParser combinedParser, bool chunk) { }
        public void OnChunkedLength(IHttpCombinedParser combinedParser, int length) { }
        public void OnChunkReceived(IHttpCombinedParser combinedParser) { }
        public void OnParserError()
        {
            HasError = true;
        }


        public void Dispose()
        {
        }
    }
}
