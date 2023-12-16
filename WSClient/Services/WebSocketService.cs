using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WSClient.Models;

namespace WSClient.Services
{
    internal class WebSocketService
    {
        public event EventHandler<Message> MessageReceived;
        ClientWebSocket ws;
        internal async Task<bool> Connect(string name = "")
        {
            try
            {
                ws = new ClientWebSocket();
                Console.WriteLine("Waiting connection");
                await ws.ConnectAsync(new Uri("ws://tefash004-001-site1.gtempurl.com:80/ws?name=" + name), CancellationToken.None);
                Console.WriteLine("Connected Successfully");
                Listening();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return false;
            }
        }

        private void Listening()
        {
            Task.Run(async () =>
            {
                try
                {
                    while (true)
                    {
                        if (ws.State != WebSocketState.Open)
                            break;
                        ArraySegment<byte> bytes = new ArraySegment<byte>(new byte[1024 * 4]);
                        var result = await ws.ReceiveAsync(bytes, CancellationToken.None);
                        if (result.MessageType == WebSocketMessageType.Close)
                        {
                            break;
                        }
                        var data = Encoding.UTF8.GetString(bytes.ToArray(), 0, result.Count);
                        Console.WriteLine(data);
                        try
                        {
                            var message = JsonSerializer.Deserialize<Message>(data);
                            if (message != null)
                                MessageReceived?.Invoke(this, message);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            });
        }

        internal async Task SendMessage(Message message, int trial = 1)
        {
            try
            {
                var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));
                if (ws.State == WebSocketState.Open)
                {
                    await ws.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
                }
                else
                {
                    await CloseConnection();
                    await Connect();
                    if (trial < 5)
                        await SendMessage(message, ++trial);
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }

        internal async Task CloseConnection()
        {
            if (ws?.State == WebSocketState.Open || ws?.State == WebSocketState.Connecting)
            {
                try
                {
                    await ws.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, "CloseConnection socket", CancellationToken.None);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
