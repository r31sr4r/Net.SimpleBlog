using Microsoft.EntityFrameworkCore;
using Net.SimpleBlog.Domain.Entity;
using Net.SimpleBlog.Infra.Data.EF.Configurations;

namespace Net.SimpleBlog.Infra.Data.EF;
public class NetSimpleBlogDbContext
    : DbContext
{
    public DbSet<User> Users => Set<User>();

    public NetSimpleBlogDbContext(
        DbContextOptions<NetSimpleBlogDbContext> options)
    : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
    }
}
