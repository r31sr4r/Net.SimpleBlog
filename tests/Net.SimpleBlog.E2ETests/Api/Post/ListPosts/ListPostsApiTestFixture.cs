using Net.SimpleBlog.Domain.SeedWork.SearchableRepository;
using Net.SimpleBlog.E2ETests.Api.Post.Common;
using DomainEntity = Net.SimpleBlog.Domain.Entity;

namespace Net.SimpleBlog.E2ETests.Api.Post.ListPosts;

[CollectionDefinition(nameof(ListPostsApiTestFixture))]
public class ListPostsApiTestFixtureCollection : ICollectionFixture<ListPostsApiTestFixture>
{ }

public class ListPostsApiTestFixture : PostBaseFixture
{
    public List<DomainEntity.Post> GetExamplePostsListWithContent(Guid userId, List<string> contents)
        => contents.Select(content => new DomainEntity.Post(
            "Example Title",
            content,
            userId
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
            ("title", SearchOrder.Asc) => listClone.OrderBy(x => x.Title),
            ("title", SearchOrder.Desc) => listClone.OrderByDescending(x => x.Title),
            ("createdAt", SearchOrder.Asc) => listClone.OrderBy(x => x.CreatedAt),
            ("createdAt", SearchOrder.Desc) => listClone.OrderByDescending(x => x.CreatedAt),
            _ => listClone.OrderBy(x => x.Title),
        };

        return orderedEnumerable
            .ThenBy(x => x.CreatedAt)
            .ToList();
    }
}
