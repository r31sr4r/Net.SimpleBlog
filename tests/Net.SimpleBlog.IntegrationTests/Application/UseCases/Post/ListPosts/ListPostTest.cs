using Net.SimpleBlog.Infra.Data.EF;
using Net.SimpleBlog.Infra.Data.EF.Repositories;
using Net.SimpleBlog.Application.UseCases.Post.ListPosts;
using UseCase = Net.SimpleBlog.Application.UseCases.Post.ListPosts;
using FluentAssertions;
using Net.SimpleBlog.Application.UseCases.Post.Common;
using Net.SimpleBlog.Domain.SeedWork.SearchableRepository;

namespace Net.SimpleBlog.IntegrationTests.Application.UseCases.Post.ListPosts;

[Collection(nameof(ListPostsTestFixture))]
public class ListPostsTest
{
    private readonly ListPostsTestFixture _fixture;

    public ListPostsTest(ListPostsTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = "SearchReturnsListAndTotal")]
    [Trait("Integration/Application", "ListPosts - Use Cases")]
    public async Task SearchReturnsListAndTotal()
    {
        NetSimpleBlogDbContext dbContext = _fixture.CreateDbContext();
        var examplePostList = _fixture.GetExamplePostsListWithTitles(new List<string> { "Title 1", "Title 2", "Title 3" });
        await dbContext.AddRangeAsync(examplePostList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var postRepository = new PostRepository(dbContext);
        var searchInput = new ListPostsInput(page: 1, perPage: 10);
        var useCase = new UseCase.ListPosts(postRepository);

        var output = await useCase.Handle(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.Page.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(examplePostList.Count);
        output.Items.Should().HaveCount(examplePostList.Count);
        foreach (PostModelOutput outputItem in output.Items)
        {
            var exampleItem = examplePostList.Find(x => x.Id == outputItem.Id);
            outputItem.Should().NotBeNull();
            outputItem!.Id.Should().Be(exampleItem!.Id);
            outputItem.Title.Should().Be(exampleItem.Title);
            outputItem.Content.Should().Be(exampleItem.Content);
            outputItem.UserId.Should().Be(exampleItem.UserId);
            outputItem.CreatedAt.Should().BeCloseTo(exampleItem.CreatedAt, TimeSpan.FromMilliseconds(100));
        }
    }

    [Fact(DisplayName = "SearchReturnsEmpty")]
    [Trait("Integration/Application", "ListPosts - Use Cases")]
    public async Task SearchReturnsEmpty()
    {
        NetSimpleBlogDbContext dbContext = _fixture.CreateDbContext();
        var postRepository = new PostRepository(dbContext);
        var searchInput = new ListPostsInput(page: 1, perPage: 10);
        var useCase = new UseCase.ListPosts(postRepository);

        var output = await useCase.Handle(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.Page.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(0);
        output.Items.Should().HaveCount(0);
    }

    [Theory(DisplayName = "SearchReturnsPaginated")]
    [Trait("Integration/Application", "ListPosts - Use Cases")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 3, 5, 0)]
    public async Task SearchReturnsPaginated(
    int itemsToGenerate,
    int page,
    int perPage,
    int expectedTotal
)
    {
        NetSimpleBlogDbContext dbContext = _fixture.CreateDbContext();
        var examplePostList = _fixture.GetExamplePostsListWithTitles(Enumerable.Range(1, itemsToGenerate).Select(i => $"Title {i}").ToList());
        await dbContext.AddRangeAsync(examplePostList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var postRepository = new PostRepository(dbContext);
        var searchInput = new ListPostsInput(page, perPage);
        var useCase = new UseCase.ListPosts(postRepository);

        var output = await useCase.Handle(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.Page.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(examplePostList.Count);
        output.Items.Should().HaveCount(expectedTotal);
        foreach (PostModelOutput outputItem in output.Items)
        {
            var exampleItem = examplePostList.Find(x => x.Id == outputItem.Id);
            outputItem.Should().NotBeNull();
            outputItem!.Id.Should().Be(exampleItem!.Id);
            outputItem.Title.Should().Be(exampleItem.Title);
            outputItem.Content.Should().Be(exampleItem.Content);
            outputItem.UserId.Should().Be(exampleItem.UserId);
            outputItem.CreatedAt.Should().BeCloseTo(exampleItem.CreatedAt, TimeSpan.FromMilliseconds(100));
        }
    }

    [Theory(DisplayName = "SearchByText")]
    [Trait("Integration/Application", "ListPosts - Use Cases")]
    [InlineData("Title 1", 1, 5, 1, 1)]
    [InlineData("Example", 1, 5, 3, 3)]
    [InlineData("Example", 2, 5, 3, 0)]
    [InlineData("Example", 3, 5, 3, 0)]
    public async Task SearchByText(
    string search,
    int page,
    int perPage,
    int expectedTotalResult,
    int expectedTotalItems
    )
    {
        NetSimpleBlogDbContext dbContext = _fixture.CreateDbContext();
        var examplePostList = _fixture.GetExamplePostsListWithTitles(
            new List<string>()
            {
                "Example Post 1",
                "Example Post 2",
                "Title 1",
                "Example Post 3",
            }
        );
        await dbContext.AddRangeAsync(examplePostList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var postRepository = new PostRepository(dbContext);
        var searchInput = new ListPostsInput(page, perPage, search);
        var useCase = new UseCase.ListPosts(postRepository);

        var output = await useCase.Handle(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.Page.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(expectedTotalResult);
        output.Items.Should().HaveCount(expectedTotalItems);
        foreach (PostModelOutput outputItem in output.Items)
        {
            var exampleItem = examplePostList.Find(x => x.Id == outputItem.Id);
            outputItem.Should().NotBeNull();
            outputItem!.Id.Should().Be(exampleItem!.Id);
            outputItem.Title.Should().Be(exampleItem.Title);
            outputItem.Content.Should().Be(exampleItem.Content);
            outputItem.UserId.Should().Be(exampleItem.UserId);
            outputItem.CreatedAt.Should().BeCloseTo(exampleItem.CreatedAt, TimeSpan.FromMilliseconds(100));
        }
    }

    [Theory(DisplayName = "SearchOrdered")]
    [Trait("Integration/Application", "ListPosts - Use Cases")]
    [InlineData("title", "asc")]
    [InlineData("title", "desc")]
    [InlineData("createdAt", "asc")]
    [InlineData("createdAt", "desc")]
    public async Task SearchOrdered(
        string orderBy,
        string order
    )
    {
        NetSimpleBlogDbContext dbContext = _fixture.CreateDbContext();
        var examplePostList = _fixture.GetExamplePostsListWithTitles(Enumerable.Range(1, 10).Select(i => $"Title {i}").ToList());
        await dbContext.AddRangeAsync(examplePostList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var postRepository = new PostRepository(dbContext);
        var searchOrder = order == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
        var searchInput = new ListPostsInput(
            page: 1,
            perPage: 20,
            "",
            orderBy,
            searchOrder
        );
        var useCase = new UseCase.ListPosts(postRepository);

        var output = await useCase.Handle(searchInput, CancellationToken.None);

        var expectOrdered = _fixture.SortList(examplePostList, orderBy, searchOrder);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.Page.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(examplePostList.Count);
        output.Items.Should().HaveCount(examplePostList.Count);

        for (int i = 0; i < output.Items.Count; i++)
        {
            var outputItem = output.Items[i];
            var exampleItem = expectOrdered[i];
            outputItem.Should().NotBeNull();
            outputItem!.Id.Should().Be(exampleItem.Id);
            outputItem.Title.Should().Be(exampleItem.Title);
            outputItem.Content.Should().Be(exampleItem.Content);
            outputItem.UserId.Should().Be(exampleItem.UserId);
            outputItem.CreatedAt.Should().BeCloseTo(exampleItem.CreatedAt, TimeSpan.FromMilliseconds(100));
        }
    }
}
