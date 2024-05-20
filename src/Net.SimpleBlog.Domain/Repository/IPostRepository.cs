using Net.SimpleBlog.Domain.Entity;
using Net.SimpleBlog.Domain.SeedWork;
using Net.SimpleBlog.Domain.SeedWork.SearchableRepository;

namespace Net.SimpleBlog.Domain.Repository
{
    public interface IPostRepository 
        : IGenericRepository<Post>, ISearchableRepository<Post>
    {
        Task<IReadOnlyList<Post>> GetPostsByUserId(Guid userId, CancellationToken cancellationToken);
    }
}
