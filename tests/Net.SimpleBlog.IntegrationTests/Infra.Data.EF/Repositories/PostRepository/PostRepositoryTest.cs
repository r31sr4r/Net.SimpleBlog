using FluentAssertions;
using Net.SimpleBlog.Application.Exceptions;
using Net.SimpleBlog.Domain.Entity;
using Net.SimpleBlog.Domain.SeedWork.SearchableRepository;
using Net.SimpleBlog.Infra.Data.EF;
using Repository = Net.SimpleBlog.Infra.Data.EF.Repositories;

namespace Net.SimpleBlog.IntegrationTests.Infra.Data.EF.Repositories.PostRepository;
[Collection(nameof(PostRepositoryTestFixture))]
public class PostRepositoryTest
{
    private readonly PostRepositoryTestFixture _fixture;

    public PostRepositoryTest(PostRepositoryTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = "Insert")]
    [Trait("Integration/Infra.Data", "PostRepository - Repositories")]
    public async Task Insert()
    {
        NetSimpleBlogDbContext dbContext = _fixture.CreateDbContext();
        var examplePost = _fixture.GetExamplePost();
        var postRepository = new Repository.PostRepository(dbContext);

        await postRepository.Insert(examplePost, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var dbPost = await (_fixture.CreateDbContext(true))
            .Posts.FindAsync(examplePost.Id);

        dbPost.Should().NotBeNull();
        dbPost.Id.Should().Be(examplePost.Id);
        dbPost.Title.Should().Be(examplePost.Title);
        dbPost.Content.Should().Be(examplePost.Content);
        dbPost.UserId.Should().Be(examplePost.UserId);
        dbPost.CreatedAt.Should().BeCloseTo(examplePost.CreatedAt, TimeSpan.FromMilliseconds(100));
    }

    [Fact(DisplayName = "Get")]
    [Trait("Integration/Infra.Data", "PostRepository - Repositories")]
    public async Task Get()
    {
        NetSimpleBlogDbContext dbContext = _fixture.CreateDbContext();
        var examplePost = _fixture.GetExamplePost();
        var examplePostList = _fixture.GetExamplePostList(15);
        examplePostList.Add(examplePost);
        await dbContext.AddRangeAsync(examplePostList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var postRepository = new Repository.PostRepository(_fixture.CreateDbContext(true));

        var dbPost = await postRepository.Get(examplePost.Id, CancellationToken.None);

        dbPost.Should().NotBeNull();
        dbPost.Id.Should().Be(examplePost.Id);
        dbPost.Title.Should().Be(examplePost.Title);
        dbPost.Content.Should().Be(examplePost.Content);
        dbPost.UserId.Should().Be(examplePost.UserId);
        dbPost.CreatedAt.Should().BeCloseTo(examplePost.CreatedAt, TimeSpan.FromMilliseconds(100));
    }

    [Fact(DisplayName = "GetThrowIfNotFound")]
    [Trait("Integration/Infra.Data", "PostRepository - Repositories")]
    public async Task GetThrowIfNotFound()
    {
        NetSimpleBlogDbContext dbContext = _fixture.CreateDbContext();
        var exampleId = Guid.NewGuid();
        var examplePostList = _fixture.GetExamplePostList(15);
        await dbContext.AddRangeAsync(examplePostList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var postRepository = new Repository.PostRepository(dbContext);

        var task = async () => await postRepository.Get(exampleId, CancellationToken.None);

        await task.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Post with id {exampleId} not found");
    }

    [Fact(DisplayName = "Update")]
    [Trait("Integration/Infra.Data", "PostRepository - Repositories")]
    public async Task Update()
    {
        NetSimpleBlogDbContext dbContext = _fixture.CreateDbContext();
        var examplePost = _fixture.GetExamplePost();
        var newPost = _fixture.GetExamplePost();
        var examplePostList = _fixture.GetExamplePostList(15);
        examplePostList.Add(examplePost);
        await dbContext.AddRangeAsync(examplePostList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var postRepository = new Repository.PostRepository(dbContext);

        examplePost.Update(newPost.Title, newPost.Content);
        await postRepository.Update(examplePost, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var dbPost = await (_fixture.CreateDbContext(true))
            .Posts.FindAsync(examplePost.Id);

        dbPost.Should().NotBeNull();
        dbPost!.Id.Should().Be(examplePost.Id);
        dbPost.Title.Should().Be(examplePost.Title);
        dbPost.Content.Should().Be(examplePost.Content);
        dbPost.UserId.Should().Be(examplePost.UserId);
        dbPost.CreatedAt.Should().Be(examplePost.CreatedAt);
        dbPost.UpdatedAt.Should().BeCloseTo((DateTime)examplePost.UpdatedAt, TimeSpan.FromMilliseconds(100));
    }

    [Fact(DisplayName = "Delete")]
    [Trait("Integration/Infra.Data", "PostRepository - Repositories")]
    public async Task Delete()
    {
        NetSimpleBlogDbContext dbContext = _fixture.CreateDbContext();
        var examplePost = _fixture.GetExamplePost();
        var examplePostList = _fixture.GetExamplePostList(15);
        examplePostList.Add(examplePost);
        await dbContext.AddRangeAsync(examplePostList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var postRepository = new Repository.PostRepository(dbContext);

        await postRepository.Delete(examplePost, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var dbPost = await (_fixture.CreateDbContext(true))
            .Posts.FindAsync(examplePost.Id);

        dbPost.Should().BeNull();
    }

    [Fact(DisplayName = "SearchReturnsEmpty")]
    [Trait("Integration/Infra.Data", "PostRepository - Repositories")]
    public async Task SearchReturnsEmpty()
    {
        NetSimpleBlogDbContext dbContext = _fixture.CreateDbContext();
        var postRepository = new Repository.PostRepository(dbContext);
        var searchInput = new SearchInput(
            page: 1,
            perPage: 10,
            search: "",
            orderBy: "",
            SearchOrder.Asc
        );

        var output = await postRepository.Search(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(0);
        output.Items.Should().HaveCount(0);
    }

    [Theory(DisplayName = "SearchReturnsPaginated")]
    [Trait("Integration/Infra.Data", "PostRepository - Repositories")]
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
        var examplePostList = _fixture.GetExamplePostList(itemsToGenerate);
        await dbContext.AddRangeAsync(examplePostList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var postRepository = new Repository.PostRepository(dbContext);
        var searchInput = new SearchInput(
            page: page,
            perPage: perPage,
            search: "",
            orderBy: "",
            SearchOrder.Asc
        );

        var output = await postRepository.Search(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(examplePostList.Count);
        output.Items.Should().HaveCount(expectedTotal);
        foreach (Post outputItem in output.Items)
        {
            var exampleItem = examplePostList.Find(x => x.Id == outputItem.Id);
            outputItem.Should().NotBeNull();
            outputItem!.Id.Should().Be(exampleItem!.Id);
            outputItem.Title.Should().Be(exampleItem.Title);
            outputItem.Content.Should().Be(exampleItem.Content);
            outputItem.UserId.Should().Be(exampleItem.UserId);
            outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        }
    }

    [Theory(DisplayName = "SearchByText")]
    [Trait("Integration/Infra.Data", "PostRepository - Repositories")]
    [InlineData("Title 1", 1, 5, 1, 1)]
    [InlineData("Example", 1, 5, 3, 3)]
    [InlineData("Example", 2, 5, 3, 0)]
    [InlineData("Example", 3, 5, 3, 0)]
    public async Task SearchByText(
    string search,
    int page,
    int perPage,
    int expectedTotalResult,
    int expectedTotalItems)
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
        var postRepository = new Repository.PostRepository(dbContext);
        var searchInput = new SearchInput(
            page,
            perPage,
            search,
            orderBy: "",
            SearchOrder.Asc
        );

        var output = await postRepository.Search(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(expectedTotalResult);
        output.Items.Should().HaveCount(expectedTotalItems);
        foreach (Post outputItem in output.Items)
        {
            var exampleItem = examplePostList.Find(x => x.Id == outputItem.Id);
            outputItem.Should().NotBeNull();
            outputItem!.Id.Should().Be(exampleItem!.Id);
            outputItem.Title.Should().Be(exampleItem.Title);
            outputItem.Content.Should().Be(exampleItem.Content);
            outputItem.UserId.Should().Be(exampleItem.UserId);
            outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        }
    }


    [Theory(DisplayName = "SearchOrdered")]
    [Trait("Integration/Infra.Data", "PostRepository - Repositories")]
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
        var examplePostList = _fixture.GetExamplePostList(10);
        await dbContext.AddRangeAsync(examplePostList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var postRepository = new Repository.PostRepository(dbContext);
        var searchOrder = order == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
        var searchInput = new SearchInput(
            page: 1,
            perPage: 20,
            search: "",
            orderBy,
            searchOrder
        );

        var output = await postRepository.Search(searchInput, CancellationToken.None);

        var expectOrdered = _fixture.SortList(examplePostList, orderBy, searchOrder);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
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
            outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        }
    }

    [Fact(DisplayName = "GetPostsByUserId")]
    [Trait("Integration/Infra.Data", "PostRepository - Repositories")]
    public async Task GetPostsByUserId()
    {
        NetSimpleBlogDbContext dbContext = _fixture.CreateDbContext();
        var userId = _fixture.GetValidUserId();
        var examplePostList = _fixture.GetExamplePostsListWithTitles(
            new List<string>()
            {
            "User Post 1",
            "User Post 2",
            "User Post 3",
            }
        );

        foreach (var post in examplePostList)
        {
            typeof(Post).GetProperty(nameof(Post.UserId))!.SetValue(post, userId);
        }

        await dbContext.AddRangeAsync(examplePostList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var postRepository = new Repository.PostRepository(dbContext);

        var output = await postRepository.GetPostsByUserId(userId, CancellationToken.None);

        output.Should().NotBeNull();
        output.Should().HaveCount(examplePostList.Count);
        foreach (Post outputItem in output)
        {
            var exampleItem = examplePostList.Find(x => x.Id == outputItem.Id);
            outputItem.Should().NotBeNull();
            outputItem!.Id.Should().Be(exampleItem!.Id);
            outputItem.Title.Should().Be(exampleItem.Title);
            outputItem.Content.Should().Be(exampleItem.Content);
            outputItem.UserId.Should().Be(exampleItem.UserId);
            outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        }
    }

}
