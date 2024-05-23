using Bogus;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Net.SimpleBlog.Infra.Data.EF;

namespace Net.SimpleBlog.E2ETests.Base;
public class BaseFixture
{
    protected Faker Faker { get; set; }

    public ApiClient ApiClient { get; set; }

    public CustomWebApplicationFactory<Program> WebAppFactory { get; set; }

    private readonly string _dbConnectionString;

    public HttpClient HttpClient { get; set; }

    public BaseFixture()
    {
        Faker = new Faker("pt_BR");
        WebAppFactory = new CustomWebApplicationFactory<Program>();
        HttpClient = WebAppFactory.CreateClient();
        ApiClient = new ApiClient(HttpClient);
        var configuration = WebAppFactory.Services.GetService(typeof(IConfiguration));
        ArgumentNullException.ThrowIfNull(configuration, nameof(configuration));
        _dbConnectionString = ((IConfiguration)configuration).GetConnectionString("NetSimpleBlogDb");
    }

    public NetSimpleBlogDbContext CreateDbContext()
    {
        var context = new NetSimpleBlogDbContext(
             new DbContextOptionsBuilder<NetSimpleBlogDbContext>()
             .UseMySql(
                 _dbConnectionString,
                 ServerVersion.AutoDetect(_dbConnectionString))
             .Options
         );
        return context;
    }

    public void CleanPersistence()
    {
        var context = CreateDbContext();
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }
}
