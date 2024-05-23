using Net.SimpleBlog.Api.ApiModels.Response;
using Net.SimpleBlog.Application.UseCases.Post.Common;
using Net.SimpleBlog.Application.UseCases.Post.CreatePost;
using Net.SimpleBlog.E2ETests.Api.Post.Common;
using FluentAssertions;
using System.Net;
using Xunit;

namespace Net.SimpleBlog.E2ETests.Api.WebSockets
{
    [Collection(nameof(WebSocketTestFixture))]
    public class WebSocketTests : IDisposable
    {
        private readonly WebSocketTestFixture _webSocketFixture;
        private readonly PostBaseFixture _postFixture;

        public WebSocketTests(WebSocketTestFixture webSocketFixture, PostBaseFixture postFixture)
        {
            _webSocketFixture = webSocketFixture;
            _postFixture = postFixture;
        }

        [Fact(DisplayName = "Should Receive WebSocket Notification When Post Created")]
        [Trait("E2E/Api", "WebSocket - Notifications")]
        public async Task ShouldReceiveWebSocketNotificationWhenPostCreated()
        {
            var validUser = _postFixture.GetValidUser();
            await _postFixture.Persistence.InsertUser(validUser);

            await _webSocketFixture.InitializeAsync();

            var input = _postFixture.GetCreatePostInput(validUser.Id);
            var (response, output) = await _postFixture.ApiClient.Post<ApiResponse<PostModelOutput>>("/posts", input);

            response.Should().NotBeNull();
            response!.StatusCode.Should().Be(HttpStatusCode.Created);

            var message = await _webSocketFixture.ReceiveWebSocketMessageAsync();
            message.Should().Contain($"New post created: {output.Data.Title}");
        }

        public void Dispose()
        {
            _webSocketFixture.Dispose();
        }
    }
}
