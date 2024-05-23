using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Net.SimpleBlog.E2ETests.Base;
using Xunit;

namespace Net.SimpleBlog.E2ETests.Api.WebSockets
{
    [CollectionDefinition(nameof(WebSocketTestFixture))]
    public class WebSocketTestFixtureCollection : ICollectionFixture<WebSocketTestFixture>
    {
    }

    public class WebSocketTestFixture : BaseFixture, IAsyncLifetime
    {
        public ClientWebSocket ClientWebSocket { get; private set; }

        public async Task InitializeAsync()
        {
            ClientWebSocket = new ClientWebSocket();
            // Certifique-se de que a URL está correta
            var wsUri = new Uri($"ws://localhost:5000/ws");
            await Task.Delay(1000); // Aguarde um tempo para garantir que o servidor esteja ativo
            await ClientWebSocket.ConnectAsync(wsUri, CancellationToken.None);
        }

        public Task DisposeAsync()
        {
            ClientWebSocket.Dispose();
            return Task.CompletedTask;
        }

        public async Task<string> ReceiveWebSocketMessageAsync()
        {
            var buffer = new byte[1024 * 4];
            var result = await ClientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            return Encoding.UTF8.GetString(buffer, 0, result.Count);
        }
    }
}
