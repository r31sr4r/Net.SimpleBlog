using Net.SimpleBlog.Domain.SeedWork.SearchableRepository;
using Net.SimpleBlog.E2ETests.Api.User.Common;
using DomainEntity = Net.SimpleBlog.Domain.Entity;

namespace Net.SimpleBlog.E2ETests.Api.User.ListUsers;

[CollectionDefinition(nameof(ListUsersApiTestFixture))]
public class ListUsersApiTestFixtureCollection
: ICollectionFixture<ListUsersApiTestFixture>
{ }

public class ListUsersApiTestFixture
    : UserBaseFixture
{
    public List<DomainEntity.User> GetExampleUsersListWithNames(List<string> names)
        => names.Select(name => new DomainEntity.User(
            name,
            GetValidEmail(),
            GetValidPhone(),
            GetValidCPF(),
            GetValidDateOfBirth(),
            GetValidRG(),
            GetValidPassword()
        )).ToList();

    public List<DomainEntity.User> SortList(
        List<DomainEntity.User> usersList,
        string orderBy,
        SearchOrder order
        )
    {
        var listClone = new List<DomainEntity.User>(usersList);
        var orderedEnumerable = (orderBy, order) switch
        {
            ("name", SearchOrder.Asc) => listClone.OrderBy(x => x.Name),
            ("name", SearchOrder.Desc) => listClone.OrderByDescending(x => x.Name),
            ("createdAt", SearchOrder.Asc) => listClone.OrderBy(x => x.CreatedAt),
            ("createdAt", SearchOrder.Desc) => listClone.OrderByDescending(x => x.CreatedAt),
            _ => listClone.OrderBy(x => x.Name),
        };

        return orderedEnumerable
            .ThenBy(x => x.CreatedAt)
            .ToList();
    }
}
