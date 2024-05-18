using Net.SimpleBlog.Application.Interfaces;
using Net.SimpleBlog.Infra.Data.EF;

namespace FC.Codeflix.Catalog.Infra.Data.EF;
public class UnitOfWork
    : IUnitOfWork
{
    private readonly NetSimpleBlogDbContext _context;

    public UnitOfWork(NetSimpleBlogDbContext context)
    {
        _context = context;
    }

    public Task Commit(CancellationToken cancellationToken)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    public Task Rollback(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
