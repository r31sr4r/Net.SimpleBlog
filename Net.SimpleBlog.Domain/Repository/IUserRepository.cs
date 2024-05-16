using Net.SimpleBlog.Domain.Entity;

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

