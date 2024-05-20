using Microsoft.EntityFrameworkCore;
using Net.SimpleBlog.Application.Exceptions;
using Net.SimpleBlog.Domain.Entity;
using Net.SimpleBlog.Domain.Repository;
using Net.SimpleBlog.Domain.SeedWork.SearchableRepository;

namespace Net.SimpleBlog.Infra.Data.EF.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly NetSimpleBlogDbContext _context;
        private DbSet<Post> _posts => _context.Set<Post>();

        public PostRepository(NetSimpleBlogDbContext context)
        {
            _context = context;
        }

        public async Task Insert(Post aggregate, CancellationToken cancellationToken)
            => await _posts.AddAsync(aggregate, cancellationToken);

        public async Task<Post> Get(Guid id, CancellationToken cancellationToken)
        {
            var post = await _posts.AsNoTracking().FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken
            );
            NotFoundException.ThrowIfNull(post, $"Post with id {id} not found");
            return post!;
        }

        public Task Update(Post aggregate, CancellationToken _)
            => Task.FromResult(_posts.Update(aggregate));

        public Task Delete(Post aggregate, CancellationToken _)
            => Task.FromResult(_posts.Remove(aggregate));

        public async Task<SearchOutput<Post>> Search(
            SearchInput input,
            CancellationToken cancellationToken)
        {
            var toSkip = (input.Page - 1) * input.PerPage;
            var query = _posts.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(input.Search))
                query = query.Where(x => x.Title.Contains(input.Search) 
                || x.Content.Contains(input.Search));

            query = AddSorting(query, input.OrderBy, input.Order);

            var total = await query.CountAsync();
            var items = await query.AsNoTracking()
                .Skip(toSkip)
                .Take(input.PerPage)
                .ToListAsync();

            return new SearchOutput<Post>(
                currentPage: input.Page,
                perPage: input.PerPage,
                total: total,
                items: items
            );
        }


        private IQueryable<Post> AddSorting(
            IQueryable<Post> query,
            string orderProperty,
            SearchOrder order
        )
        {
            var orderedEnumerable = (orderProperty, order) switch
            {
                ("title", SearchOrder.Asc) => query.OrderBy(x => x.Title),
                ("title", SearchOrder.Desc) => query.OrderByDescending(x => x.Title),
                ("createdAt", SearchOrder.Asc) => query.OrderBy(x => x.CreatedAt),
                ("createdAt", SearchOrder.Desc) => query.OrderByDescending(x => x.CreatedAt),
                _ => query.OrderBy(x => x.Title)
            };

            return orderedEnumerable
                .ThenBy(x => x.CreatedAt);
        }

        public async Task<IReadOnlyList<Post>> GetPostsByUserId(Guid userId, CancellationToken cancellationToken)
        {
            return await _posts.AsNoTracking()
                .Where(p => p.UserId == userId)
                .ToListAsync(cancellationToken);
        }
    }
}
