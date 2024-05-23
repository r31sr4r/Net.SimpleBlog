using Net.SimpleBlog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;
using DomainEntity = Net.SimpleBlog.Domain.Entity;

namespace Net.SimpleBlog.E2ETests.Api.Post.Common;
public class PostPersistence
{
    private readonly NetSimpleBlogDbContext _context;

    public PostPersistence(NetSimpleBlogDbContext context)
        => _context = context;

    public async Task<DomainEntity.Post?> GetPostById(Guid id)
        => await _context
            .Posts
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

    public async Task InsertPosts(List<DomainEntity.Post> posts)
    {
        await _context.Posts.AddRangeAsync(posts);
        await _context.SaveChangesAsync();
    }

    public async Task InsertUser(DomainEntity.User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }
}
