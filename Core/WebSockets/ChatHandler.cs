using System;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Threading;
namespace ClearServer.Core.WebSockets
{
    public static class ChatHandler
    {
        private static readonly List<WebSocket> Clients = new List<WebSocket>();
        private static readonly ReaderWriterLockSlim Locker = new ReaderWriterLockSlim();

        public static async void ChatConnection(HttpListenerContext context)
        {
            var socketContext = await context.AcceptWebSocketAsync(null);
            var socket = socketContext.WebSocket;
            Locker.EnterWriteLock();
            try
            {
                Clients.Add(socket);
            }
            finally
            {
                Locker.ExitWriteLock();
            }

            while (socket.State == WebSocketState.Open)
            {
                var buffer = new ArraySegment<byte>(new byte[1024]);
                await socket.ReceiveAsync(buffer, CancellationToken.None);
                for (var i = 0; i < Clients.Count; i++)
                {
                    var client = Clients[i];

                    try
                    {
                        if (client.State == WebSocketState.Open)
                        {

                            await client.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                        }
                    }
                    catch (Exception)
                    {
                        Locker.EnterWriteLock();
                        try
                        {
                            Clients.Remove(client);
                            i--;
                        }
                        finally
                        {
                            Locker.ExitWriteLock();
                        }
                    }
                }
            }
        }
    }
}
