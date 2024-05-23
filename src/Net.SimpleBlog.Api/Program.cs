using Microsoft.Extensions.FileProviders;
using Net.SimpleBlog.Api.Configurations;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddLoggingConfiguration();

builder.Services
    .AddAppConnections(builder.Configuration)
    .AddUseCases()
    .AddAndConfigureControllers()
    .AddSecurityServices(builder.Configuration)
    .AddWebSocketSupport();

builder.Logging.AddConsole();

var app = builder.Build();

app.Lifetime.ApplicationStarted.Register(() => Log.Information("Application started"));
app.Lifetime.ApplicationStopping.Register(() => Log.Information("Application is stopping"));
app.Lifetime.ApplicationStopped.Register(() => Log.Information("Application stopped"));

Log.Information("Starting application setup...");

app.UseDocumentation();
app.UseHttpsRedirection();

app.UseWebSocketSupport();
Log.Information("WebSocket Support Enabled");

app.UseAuthorization();
app.MapControllers();

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "WebSockets")),
    RequestPath = "/websockets"
});

Log.Information("Static file serving enabled for path /websockets");

app.Run();

public partial class Program { }
