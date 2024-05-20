using Net.SimpleBlog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;
using DomainEntity = Net.SimpleBlog.Domain.Entity;


namespace Net.SimpleBlog.E2ETests.Api.User.Common;
public class UserPersistence
{
    private readonly NetSimpleBlogDbContext _context;

    public UserPersistence(NetSimpleBlogDbContext context)
        => _context = context;

    public async Task<DomainEntity.User?> GetById(Guid id)
        => await _context
            .Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task InsertList(List<DomainEntity.User> users)
    {
        await _context.Users.AddRangeAsync(users);
        await _context.SaveChangesAsync();
    }
}
