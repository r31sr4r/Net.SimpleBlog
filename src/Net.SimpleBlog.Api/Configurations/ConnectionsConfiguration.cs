using Net.SimpleBlog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;

namespace Net.SimpleBlog.Api.Configurations;

public static class ConnectionsConfiguration
{
    public static IServiceCollection AddAppConnections(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddDbConnection(configuration);
        return services;
    }

    private static IServiceCollection AddDbConnection(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        var connectionString = configuration.GetConnectionString("NetSimpleBlogDb");
        services.AddDbContext<NetSimpleBlogDbContext>(
            options => options.UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString)
            )
        );

        return services;
    }

    public static WebApplication MigrateDatabase(
       this WebApplication app)
    {
        var environment = Environment
            .GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        if (environment == "E2ETest") return app;
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider
            .GetRequiredService<NetSimpleBlogDbContext>();
        dbContext.Database.Migrate();
        return app;
    }
}
