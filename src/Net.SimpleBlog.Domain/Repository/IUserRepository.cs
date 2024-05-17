using Net.SimpleBlog.Domain.Entity;
using Net.SimpleBlog.Domain.SeedWork;
using Net.SimpleBlog.Domain.SeedWork.SearchableRepository;

namespace Net.SimpleBlog.Domain.Repository;
public interface IUserRepository
    : IGenericRepository<User>,
    ISearchableRepository<User>
{
    public Task<IReadOnlyList<Guid>> GetIdsListByIds(
        List<Guid> ids,
        CancellationToken cancellationToken
    );

    Task<User> GetByEmail(string email, CancellationToken cancellationToken);

}

