using Bogus;
using Net.SimpleBlog.Application.UseCases.Post.ListPosts;
using Net.SimpleBlog.Domain.SeedWork.SearchableRepository;
using Net.SimpleBlog.UnitTests.Application.Post.Common;
using Xunit;
using DomainEntity = Net.SimpleBlog.Domain.Entity;

namespace Net.SimpleBlog.UnitTests.Application.Post.ListPosts;

[CollectionDefinition(nameof(ListPostsTestFixture))]
public class ListPostsTestFixtureCollection
    : ICollectionFixture<ListPostsTestFixture>
{ }

public class ListPostsTestFixture
    : PostUseCasesBaseFixture
{
    public List<DomainEntity.Post> GetPostsList(int length = 10)
    {
        var list = new List<DomainEntity.Post>();
        for (int i = 0; i < length; i++)
        {
            list.Add(GetValidPost());
        }
        return list;
    }

    public ListPostsInput GetInput()
    {
        var random = new Random();
        return new ListPostsInput(
            page: random.Next(1, 10),
            perPage: random.Next(15, 100),
            search: Faker.Lorem.Sentence(),
            sort: Faker.Lorem.Word(),
            dir: random.Next(0, 10) > 5 ? SearchOrder.Asc : SearchOrder.Desc
        );
    }
}
