using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Net.SimpleBlog.Api.WebSockets;

namespace Net.SimpleBlog.Api.Configurations
{
    public static class WebSocketConfiguration
    {
        public static IServiceCollection AddWebSocketSupport(this IServiceCollection services)
        {
            services.AddSingleton<WebSocketConnectionManager>();
            return services;
        }

        public static IApplicationBuilder UseWebSocketSupport(this IApplicationBuilder app)
        {
            var logger = app.ApplicationServices.GetRequiredService<ILoggerFactory>().CreateLogger("WebSocketSupport");

            app.UseWebSockets();
            app.UseWebSocketEndpoint(logger);

            return app;
        }

        private static void UseWebSocketEndpoint(this IApplicationBuilder app, ILogger logger)
        {
            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/ws")
                {
                    await HandleWebSocketRequest(context, logger);
                }
                else
                {
                    await next();
                }
            });
        }

        private static async Task HandleWebSocketRequest(HttpContext context, ILogger logger)
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                logger.LogInformation("WebSocket request received at {Path}", context.Request.Path);
                var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                var webSocketManager = context.RequestServices.GetRequiredService<WebSocketConnectionManager>();
                logger.LogInformation("Accepted WebSocket connection from {RemoteIpAddress}", context.Connection.RemoteIpAddress);
                await webSocketManager.HandleWebSocketAsync(context, webSocket);
            }
            else
            {
                logger.LogWarning("Rejected non-WebSocket request to {Path}", context.Request.Path);
                context.Response.StatusCode = 400;
            }
        }
    }
}
