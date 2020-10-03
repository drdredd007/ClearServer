using System;
using System.Collections.Generic;
using System.Text;

namespace ClearServerCore.Core.WebSockets.ChatController
{
    public class ChatMessage
    {
        public int mid { get; set; }
        public string from_User { get; set; }
        public string to_User { get; set; }
        public string message { get; set; }
        public DateTime timeStamp { get; set; }
        public bool isRead { get; set; }

        public override string ToString()
        {
            return $"User {from_User} send message {message} to {to_User}";
        }
    }
}
