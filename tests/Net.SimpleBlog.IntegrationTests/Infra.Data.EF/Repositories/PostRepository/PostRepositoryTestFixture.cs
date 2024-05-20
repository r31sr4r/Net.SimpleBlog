using Bogus;
using Net.SimpleBlog.Domain.Entity;
using Net.SimpleBlog.Domain.SeedWork.SearchableRepository;
using Net.SimpleBlog.IntegrationTests.Base;

namespace Net.SimpleBlog.IntegrationTests.Infra.Data.EF.Repositories.PostRepository;

[CollectionDefinition(nameof(PostRepositoryTestFixture))]
public class PostRepositoryTestFixtureCollection
    : ICollectionFixture<PostRepositoryTestFixture>
{ }

public class PostRepositoryTestFixture
    : BaseFixture
{
    private readonly Faker _faker;

    public PostRepositoryTestFixture()
    {
        _faker = new Faker("pt_BR");
    }

    public string GetValidTitle()
        => _faker.Lorem.Sentence();

    public string GetValidContent()
        => _faker.Lorem.Paragraphs();

    public Guid GetValidUserId()
        => Guid.NewGuid();

    public DateTime GetValidDate()
        => _faker.Date.Past();

    public Post GetExamplePost()
        => new(
            GetValidTitle(),
            GetValidContent(),
            GetValidUserId()
        );

    public List<Post> GetExamplePostList(int length = 10)
        => Enumerable.Range(1, length)
            .Select(_ => GetExamplePost()).ToList();

    public List<Post> GetExamplePostsListWithTitles(List<string> titles)
        => titles.Select(title => new Post(
            title,
            GetValidContent(),
            GetValidUserId()
        )).ToList();

    public List<Post> SortList(
        List<Post> postsList,
        string orderBy,
        SearchOrder order
    )
    {
        var listClone = new List<Post>(postsList);
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
