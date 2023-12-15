using System.Net.WebSockets;
using System.Text;

ClientWebSocket ws = new ClientWebSocket();
Console.WriteLine("Waiting connection");
await ws.ConnectAsync(new Uri("ws://localhost:5140/ws"), CancellationToken.None);
Console.WriteLine("Connected Successfully");
await Task.Run(async () =>
{
    var buffer = new byte[1024];
    while (true)
    {
        if (ws.State != WebSocketState.Open)
            break;
        var result = await ws.ReceiveAsync(buffer, CancellationToken.None);
        if (result.MessageType == WebSocketMessageType.Close)
        {
            break;
        }
        var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
        Console.WriteLine(message);
    }
});