using Net.SimpleBlog.Api.ApiModels.Response;
using Net.SimpleBlog.Application.UseCases.Post.Common;
using Net.SimpleBlog.Application.UseCases.Post.CreatePost;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Net.SimpleBlog.E2ETests.Api.Post.CreatePost;

[Collection(nameof(CreatePostApiTestFixture))]
public class CreatePostApiTest : IDisposable
{
    private readonly CreatePostApiTestFixture _fixture;

    public CreatePostApiTest(CreatePostApiTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(CreatePost))]
    [Trait("E2E/Api", "Post/Create - Endpoints")]
    public async Task CreatePost()
    {
        var validUser = _fixture.GetValidUser();
        await _fixture.Persistence.InsertUser(validUser);

        var input = _fixture.GetInput(validUser.Id);

        var (response, output) = await _fixture
            .ApiClient
            .Post<ApiResponse<PostModelOutput>>(
                "/posts",
                input
            );

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.Created);
        output!.Data.Should().NotBeNull();
        output.Data.Title.Should().Be(input.Title);
        output.Data.Content.Should().Be(input.Content);
        output.Data.UserId.Should().Be(input.UserId);
        output.Data.Id.Should().NotBeEmpty();
        output.Data.CreatedAt.Should().NotBeSameDateAs(default);

        var dbPost = await _fixture.Persistence
            .GetPostById(output.Data.Id);
        dbPost.Should().NotBeNull();
        dbPost!.Title.Should().Be(input.Title);
        dbPost.Content.Should().Be(input.Content);
        dbPost.UserId.Should().Be(input.UserId);
        dbPost.Id.Should().NotBeEmpty();
    }

    [Theory(DisplayName = nameof(ErrorWhenCantInstantiateAggregate))]
    [Trait("E2E/Api", "Post/Create - Endpoints")]
    [MemberData(
        nameof(CreatePostApiTestDataGenerator.GetInvalidInputs),
        MemberType = typeof(CreatePostApiTestDataGenerator)
    )]
    public async Task ErrorWhenCantInstantiateAggregate(
        CreatePostInput input,
        string expectedDetail
    )
    {
        var validUser = _fixture.GetValidUser();
        await _fixture.Persistence.InsertUser(validUser);
        input.UserId = validUser.Id;  

        var (response, output) = await _fixture
            .ApiClient
            .Post<ProblemDetails>(
                "/posts",
                input
            );

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        output.Should().NotBeNull();
        output!.Title.Should().Be("One or more validation errors occurred");
        output.Type.Should().Be("UnprocessableEntity");
        output.Status.Should().Be((int)StatusCodes.Status422UnprocessableEntity);
        output.Detail.Should().Be(expectedDetail);
    }

    public void Dispose()
        => _fixture.CleanPersistence();
}
