using Net.SimpleBlog.Infra.Data.EF;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Net.SimpleBlog.E2ETests.Base
{
    public class CustomWebApplicationFactory<TStartup>
        : WebApplicationFactory<TStartup>, IDisposable
        where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var environment = "E2ETest";
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", environment);
            builder.UseEnvironment(environment);
            builder.UseUrls("http://localhost:5000"); // Certifique-se de que a URL correta está sendo usada
            builder.ConfigureServices(services =>
            {
                var serviceProvider = services.BuildServiceProvider();
                using (var scope = serviceProvider.CreateScope())
                {
                    var context = scope.ServiceProvider.GetService<NetSimpleBlogDbContext>();
                    ArgumentNullException.ThrowIfNull(context);
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();
                }
            });

            base.ConfigureWebHost(builder);
        }
    }
}
