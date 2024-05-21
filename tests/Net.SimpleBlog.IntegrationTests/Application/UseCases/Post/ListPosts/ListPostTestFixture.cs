using Net.SimpleBlog.Domain.SeedWork.SearchableRepository;
using Net.SimpleBlog.IntegrationTests.Application.UseCases.Post.Common;
using DomainEntity = Net.SimpleBlog.Domain.Entity;

namespace Net.SimpleBlog.IntegrationTests.Application.UseCases.Post.ListPosts;

[CollectionDefinition(nameof(ListPostsTestFixture))]
public class ListPostsTestFixtureCollection
    : ICollectionFixture<ListPostsTestFixture>
{ }

public class ListPostsTestFixture
    : PostUseCasesBaseFixture
{

    public List<DomainEntity.Post> GetExamplePostsListWithTitles(List<string> titles)
    => titles.Select(title => new DomainEntity.Post(
        title,
        GetValidContent(),
        GetValidUserId()
    )).ToList();

    public List<DomainEntity.Post> SortList(
    List<DomainEntity.Post> postsList,
    string orderBy,
    SearchOrder order
)
    {
        var listClone = new List<DomainEntity.Post>(postsList);
        var orderedEnumerable = (orderBy, order) switch
        {
            ("title", SearchOrder.Asc) => listClone.OrderBy(x => x.Title).ToList(),
            ("title", SearchOrder.Desc) => listClone.OrderByDescending(x => x.Title).ToList(),
            ("createdAt", SearchOrder.Asc) => listClone.OrderBy(x => x.CreatedAt).ToList(),
            ("createdAt", SearchOrder.Desc) => listClone.OrderByDescending(x => x.CreatedAt).ToList(),
            _ => listClone.OrderBy(x => x.Title).ToList(),
        };

        return orderedEnumerable.ToList();
    }
}
