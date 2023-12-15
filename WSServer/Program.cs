using System.Net.WebSockets;
using System.Net;
using System.Text.Json;
using System.Text;

namespace WSServer
{
    internal record Message(string Sender, string Text,DateTime Date);
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            app.UseWebSockets();
            app.Map("/", async context => await context.Response.WriteAsync("hello world"));
            var clients = new List<WebSocket>();
            app.Map("/ws", async context =>
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    var name = context.Request.Query["name"];
                    var ws = await context.WebSockets.AcceptWebSocketAsync();
                    clients.Add(ws);
                    await Broadcast(new Message("Server", "New member joint!\nSay hello to : " + name,DateTime.Now));
                    await ReceiveMessages(ws, async (result, bytes) =>
                    {
                        if (result != null && result.MessageType == WebSocketMessageType.Text)
                            await BroadcastBytes(bytes);
                    }, async () =>
                    {
                        await Broadcast(new Message("Server", "Member disjoint : " + name, DateTime.Now));
                    });
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                }
            });
            app.Run();

            async Task Broadcast(Message message)
            {
                var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
                await BroadcastBytes(bytes);

            }
            async Task BroadcastBytes(byte[] message)
            {
                foreach (var ws in clients)
                {
                    if (ws.State == WebSocketState.Open)
                    {
                        await ws.SendAsync(message, WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                    else
                    {
                        await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                        clients.Remove(ws);
                    }
                }
            }
            async Task ReceiveMessages(WebSocket ws, Action<WebSocketReceiveResult, byte[]> handleMessage, Action handleConnectionClosed = default)
            {
                ArraySegment<byte> bytes = new ArraySegment<byte>(new byte[1024 * 4]);
                while (ws.State == WebSocketState.Open)
                {
                    try
                    {
                        var result = await ws.ReceiveAsync(bytes, CancellationToken.None);
                        if (result.MessageType == WebSocketMessageType.Text)
                        {
                            var list = bytes.ToList();
                            list.Reverse();
                            list.RemoveRange(0, list.FindIndex(x => x > 0));
                            list.Reverse();
                            handleMessage(result, list.ToArray());
                        }
                        else if (result.MessageType == WebSocketMessageType.Close)
                        {
                            await ws.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "Server close", CancellationToken.None);
                            clients.Remove(ws);
                            if (handleConnectionClosed != null)
                            {
                                handleConnectionClosed();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        clients.Remove(ws);
                    }
                }
            }
        }

    }
}
