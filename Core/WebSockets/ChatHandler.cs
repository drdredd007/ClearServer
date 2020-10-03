using ClearServer.Core.Requester;
using ClearServerCore.Core.Database;
using ClearServerCore.Core.WebSockets.ChatController;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using Encoding = System.Text.Encoding;

namespace ClearServer.Core.WebSockets
{
    public class ChatHandler
    {
        WebSocket Socket;
        ChatUser ChatUser;
        DatabaseWorker databaseWorker = DatabaseWorker.GetInstance();
        public async void ChatConnection(HttpListenerContext context, ClientHandler handler)
        {
            var socketContext = await context.AcceptWebSocketAsync(null);
            Socket = socketContext.WebSocket;
            ChatUser = new ChatUser(handler.CurrentUser, Socket);
            ChatObserver.UsersCollection.Add(ChatUser);
            databaseWorker.OnMessageReceived += OnMessageReceived;
            while (Socket.State == WebSocketState.Open)
            {
                try
                {
                    var inputBuffer = new ArraySegment<byte>(new byte[1024]);
                    await Socket.ReceiveAsync(inputBuffer, CancellationToken.None);
                    Console.WriteLine(Encoding.UTF8.GetString(inputBuffer));
                    var msg = JsonConvert.DeserializeObject<ChatMessage>(Encoding.UTF8.GetString(inputBuffer));
                    msg.from_User = ChatUser.ClientInfo.login;
                    Console.WriteLine(msg.ToString());
                    databaseWorker.MessageProcess(msg, ChatUser.ClientInfo);
                }
                catch (Exception e)
                {
                    await Socket.CloseOutputAsync(WebSocketCloseStatus.Empty, null, CancellationToken.None);
                    Console.WriteLine(e.Message);
                }

            }
            ClearObject();
        }

        private async void ClearObject()
        {
            databaseWorker.OnMessageReceived -= OnMessageReceived;
            await Socket.CloseOutputAsync(WebSocketCloseStatus.Empty, null, CancellationToken.None);
            ChatObserver.UsersCollection.Remove(ChatUser);
            Console.WriteLine($"User {ChatUser.ClientInfo.login} disconnected");
            GC.Collect();
        }
        private async void OnMessageReceived(ChatMessage message)
        {

            switch (message)
            {
                case { } when message.from_User == ChatUser.ClientInfo.login:
                case { } when message.to_User == ChatUser.ClientInfo.login:
                    var json = JsonConvert.SerializeObject(message, Formatting.None);
                    var outputBuffer = Encoding.UTF8.GetBytes(json);
                    Console.WriteLine(json);
                    await Socket.SendAsync(outputBuffer, WebSocketMessageType.Text, true, CancellationToken.None);
                    break;
            }

        }
    }

    public class ChatUser
    {
        public WebSocket webSocket { get; set; }
        public User ClientInfo { get; set; }

        public ChatUser(User client, WebSocket webSocket)
        {
            this.ClientInfo = client;
            this.webSocket = webSocket;
        }

    }


    public static class ChatObserver
    {
        public static List<ChatUser> UsersCollection = new List<ChatUser>();
    }
}
