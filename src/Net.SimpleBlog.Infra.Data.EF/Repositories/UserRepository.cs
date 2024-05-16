using Microsoft.EntityFrameworkCore;
using Net.SimpleBlog.Domain.Entity;
using Net.SimpleBlog.Domain.Repository;
using Net.SimpleBlog.Domain.SeedWork.SearchableRepository;

namespace Net.SimpleBlog.Infra.Data.EF.Repositories;
public class UserRepository
    : IUserRepository
{
    private readonly NetSimpleBlogDbContext _context;
    private DbSet<User> _users => _context.Set<User>();

    public UserRepository(NetSimpleBlogDbContext context)
    {
        _context = context;
    }

    public async Task Insert(User aggregate, CancellationToken cancellationToken)
        => await _users.AddAsync(aggregate, cancellationToken);

    public async Task<User> Get(Guid id, CancellationToken cancellationToken)
    {
        var user = await _users.AsNoTracking().FirstOrDefaultAsync(
            x => x.Id == id,
            cancellationToken
        );
        NotFoundException.ThrowIfNull(user, $"User with id {id} not found");
        return user!;
    }

    public Task Update(User aggregate, CancellationToken _)
        => Task.FromResult(_users.Update(aggregate));

    public Task Delete(User aggregate, CancellationToken _)
        => Task.FromResult(_users.Remove(aggregate));

    public async Task<SearchOutput<User>> Search(
        SearchInput input,
        CancellationToken cancellationToken)
    {
        var toSkip = (input.Page - 1) * input.PerPage;
        var query = _users.AsNoTracking();
        query = AddSorting(query, input.OrderBy, input.Order);
        if (!string.IsNullOrWhiteSpace(input.Search))
            query = query.Where(x => x.Name.Contains(input.Search));

        var total = await query.CountAsync();
        var items = await query.AsNoTracking()
            .Skip(toSkip)
            .Take(input.PerPage)
            .ToListAsync();

        return new SearchOutput<User>(
            currentPage: input.Page,
            perPage: input.PerPage,
            total: total,
            items: items
        );
    }

    private IQueryable<User> AddSorting(
        IQueryable<User> query,
        string orderProperty,
        SearchOrder order
    )
    {
        var orderedEnumerable = (orderProperty, order) switch
        {
            ("name", SearchOrder.Asc) => query.OrderBy(x => x.Name),
            ("name", SearchOrder.Desc) => query.OrderByDescending(x => x.Name),
            ("createdAt", SearchOrder.Asc) => query.OrderBy(x => x.CreatedAt),
            ("createdAt", SearchOrder.Desc) => query.OrderByDescending(x => x.CreatedAt),
            _ => query.OrderBy(x => x.Name)
        };

        return orderedEnumerable
            .ThenBy(x => x.CreatedAt);

    }


    public async Task<User> GetByEmail(string email, CancellationToken cancellationToken)
    {
        var user = await _users.AsNoTracking().FirstOrDefaultAsync(
            x => x.Email == email,
            cancellationToken
        );
        NotFoundException.ThrowIfNull(user, $"User with email {email} not found");
        return user!;
    }

    public Task<IReadOnlyList<Guid>> GetIdsListByIds(List<Guid> ids, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }



}

