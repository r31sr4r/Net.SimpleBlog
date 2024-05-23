using Net.SimpleBlog.Application.UseCases.Post.Common;
using Net.SimpleBlog.Application.UseCases.Post.ListPosts;
using Net.SimpleBlog.Domain.SeedWork.SearchableRepository;
using Net.SimpleBlog.E2ETests.Extensions.DateTime;
using Net.SimpleBlog.E2ETests.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Net.SimpleBlog.E2ETests.Api.Post.ListPosts;

[Collection(nameof(ListPostsApiTestFixture))]
public class ListPostsApiTest : IDisposable
{
    private readonly ListPostsApiTestFixture _fixture;

    public ListPostsApiTest(ListPostsApiTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(ListPostAndTotalByDefault))]
    [Trait("E2E/Api", "Post/List - Endpoints")]
    public async Task ListPostAndTotalByDefault()
    {
        var defaultPerPage = 15;

        // Inserir um usuário válido
        var validUser = _fixture.GetValidUser();
        await _fixture.Persistence.InsertUser(validUser);

        var examplePostsList = _fixture.GetPostsList(validUser.Id, 20);
        await _fixture.Persistence.InsertPosts(examplePostsList);

        var (response, output) = await _fixture
            .ApiClient
            .Get<TestApiResponseList<PostModelOutput>>($"/posts");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta.Total.Should().Be(examplePostsList.Count);
        output.Data.Should().HaveCount(defaultPerPage);
        output!.Data.Should().NotBeNull();
        foreach (PostModelOutput post in output.Data)
        {
            var dbPost = examplePostsList
                .FirstOrDefault(x => x.Id == post.Id);
            dbPost.Should().NotBeNull();
            dbPost!.Title.Should().Be(post.Title);
            dbPost.Content.Should().Be(post.Content);
            dbPost.UserId.Should().Be(post.UserId);
            dbPost.CreatedAt.TrimMilliSeconds().Should().BeSameDateAs(
                post.CreatedAt.TrimMilliSeconds()
            );
        }
    }

    [Fact(DisplayName = nameof(ItemsEmptyWhenPersistenceEmpty))]
    [Trait("E2E/Api", "Post/List - Endpoints")]
    public async Task ItemsEmptyWhenPersistenceEmpty()
    {
        var (response, output) = await _fixture
            .ApiClient
            .Get<TestApiResponseList<PostModelOutput>>("/posts");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Should().HaveCount(0);
        output.Meta.Should().NotBeNull();
        output.Meta.Total.Should().Be(0);
        output.Data.Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(ListPostsAndTotal))]
    [Trait("E2E/Api", "Post/List - Endpoints")]
    public async Task ListPostsAndTotal()
    {
        var validUser = _fixture.GetValidUser();
        await _fixture.Persistence.InsertUser(validUser);

        var examplePostsList = _fixture.GetPostsList(validUser.Id, 20);
        await _fixture.Persistence.InsertPosts(examplePostsList);
        var input = new ListPostsInput
        {
            Page = 1,
            PerPage = 5
        };

        var (response, output) = await _fixture
            .ApiClient
            .Get<TestApiResponseList<PostModelOutput>>("/posts", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Meta.Total.Should().Be(examplePostsList.Count);
        foreach (var item in output!.Data)
        {
            var examplePost = examplePostsList
                .FirstOrDefault(x => x.Id == item.Id);
            item.Title.Should().Be(examplePost!.Title);
            item.Content.Should().Be(examplePost!.Content);
            item.UserId.Should().Be(examplePost!.UserId);
            item.CreatedAt.TrimMilliSeconds().Should().BeSameDateAs(
                examplePost!.CreatedAt.TrimMilliSeconds()
            );
        }
    }

    [Theory(DisplayName = "ListPaginated")]
    [Trait("E2E/Api", "Post/List - Endpoints")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 3, 5, 0)]
    public async Task ListPaginated(
        int itemsToGenerate,
        int page,
        int perPage,
        int expectedTotal
        )
    {
        var validUser = _fixture.GetValidUser();
        await _fixture.Persistence.InsertUser(validUser);

        var examplePostsList = _fixture.GetPostsList(validUser.Id, itemsToGenerate);
        await _fixture.Persistence.InsertPosts(examplePostsList);
        var input = new ListPostsInput
        {
            Page = page,
            PerPage = perPage
        };

        var (response, output) = await _fixture
            .ApiClient
            .Get<TestApiResponseList<PostModelOutput>>("/posts", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Meta.Total.Should().Be(examplePostsList.Count);
        foreach (var item in output!.Data)
        {
            var examplePost = examplePostsList
                .FirstOrDefault(x => x.Id == item.Id);
            item.Title.Should().Be(examplePost!.Title);
            item.Content.Should().Be(examplePost!.Content);
            item.UserId.Should().Be(examplePost!.UserId);
            item.CreatedAt.TrimMilliSeconds().Should().BeSameDateAs(
                examplePost!.CreatedAt.TrimMilliSeconds()
            );
        }
    }

    [Theory(DisplayName = "SearchByText")]
    [Trait("E2E/Api", "Post/List - Endpoints")]
    [InlineData("Title 1", 1, 5, 1, 1)]
    [InlineData("Content 2", 1, 5, 2, 2)]
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
        var validUser = _fixture.GetValidUser();
        await _fixture.Persistence.InsertUser(validUser);

        var examplePostsList = _fixture.GetExamplePostsListWithContent(
            validUser.Id,
            new List<string>()
            {
                "Example Post 1",
                "Example Post 2",
                "Title 1",
                "Content 2",
                "Example Post 3",
            }
        );

        await _fixture.Persistence.InsertPosts(examplePostsList);
        var input = new ListPostsInput
        {
            Page = page,
            PerPage = perPage,
            Search = search
        };

        var (response, output) = await _fixture
            .ApiClient
            .Get<TestApiResponseList<PostModelOutput>>("/posts", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        foreach (var item in output!.Data)
        {
            var examplePost = examplePostsList
                .FirstOrDefault(x => x.Id == item.Id);
            item.Title.Should().Be(examplePost!.Title);
            item.Content.Should().Be(examplePost!.Content);
            item.UserId.Should().Be(examplePost!.UserId);
            item.CreatedAt.TrimMilliSeconds().Should().BeSameDateAs(
                examplePost!.CreatedAt.TrimMilliSeconds()
            );
        }
    }

    [Theory(DisplayName = "SearchOrdered")]
    [Trait("E2E/Api", "Post/List - Endpoints")]
    [InlineData("title", "asc")]
    [InlineData("title", "desc")]
    [InlineData("createdAt", "asc")]
    [InlineData("createdAt", "desc")]
    public async Task SearchOrdered(
        string orderBy,
        string order
    )
    {
        var validUser = _fixture.GetValidUser();
        await _fixture.Persistence.InsertUser(validUser);

        var examplePostsList = _fixture.GetPostsList(validUser.Id, 10);
        await _fixture.Persistence.InsertPosts(examplePostsList);
        var inputOrder = order == "asc"
            ? SearchOrder.Asc
            : SearchOrder.Desc;

        var input = new ListPostsInput
        {
            Page = 1,
            PerPage = 20,
            Search = "",
            Sort = orderBy,
            Dir = inputOrder
        };

        var (response, output) = await _fixture
            .ApiClient
            .Get<TestApiResponseList<PostModelOutput>>("/posts", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Meta.Total.Should().Be(examplePostsList.Count);
        var expectOrdered = _fixture.SortList(examplePostsList, input.Sort, input.Dir);

        for (int i = 0; i < output!.Data.Count; i++)
        {
            var outputItem = output.Data[i];
            var exampleItem = expectOrdered[i];
            outputItem.Should().NotBeNull();
            outputItem.Title.Should().Be(exampleItem.Title);
            outputItem.Content.Should().Be(exampleItem.Content);
            outputItem.UserId.Should().Be(exampleItem.UserId);
            outputItem.CreatedAt.TrimMilliSeconds().Should().BeSameDateAs(
                exampleItem.CreatedAt.TrimMilliSeconds()
            );
        }
    }

    public void Dispose()
        => _fixture.CleanPersistence();
}
