using Bogus;
using Microsoft.EntityFrameworkCore;
using Net.SimpleBlog.Infra.Data.EF;

namespace Net.SimpleBlog.IntegrationTests.Base;
public class BaseFixture
{
    protected Faker Faker { get; set; }

    public BaseFixture()
    {
        Faker = new Faker("pt_BR");
    }

    public NetSimpleBlogDbContext CreateDbContext(
        bool preserveData = false
    )
    {
        var context = new NetSimpleBlogDbContext(
            new DbContextOptionsBuilder<NetSimpleBlogDbContext>()
                .UseInMemoryDatabase("integration-tests-db")
                .Options
        );

        if (!preserveData)
            context.Database.EnsureDeleted();

        return context;

    }


}