using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace Net.SimpleBlog.Api.WebSockets
{
    public class WebSocketConnectionManager
    {
        private ConcurrentDictionary<string, WebSocket> _connections = new ConcurrentDictionary<string, WebSocket>();
        private readonly ILogger<WebSocketConnectionManager> _logger;

        public WebSocketConnectionManager(ILogger<WebSocketConnectionManager> logger)
        {
            _logger = logger;
        }

        public async Task HandleWebSocketAsync(HttpContext context, WebSocket webSocket)
        {
            var connectionId = Guid.NewGuid().ToString();
            _connections[connectionId] = webSocket;
            _logger.LogInformation("WebSocket connection established with ID: {ConnectionId}", connectionId);

            await ReceiveMessagesAsync(webSocket, async (result, buffer) =>
            {
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    _connections.TryRemove(connectionId, out _);
                    _logger.LogInformation("WebSocket connection closed with ID: {ConnectionId}", connectionId);
                    await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                }
            });
        }

        public async Task BroadcastMessageAsync(string message)
        {
            _logger.LogInformation("Broadcasting message: {Message}", message);
            foreach (var socket in _connections.Values)
            {
                if (socket.State == WebSocketState.Open)
                {
                    var buffer = Encoding.UTF8.GetBytes(message);
                    await socket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }

        private async Task ReceiveMessagesAsync(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
        {
            var buffer = new byte[1024 * 4];
            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                handleMessage(result, buffer);
                _logger.LogInformation("Received message of type: {MessageType}", result.MessageType);
            }
        }
    }
}
