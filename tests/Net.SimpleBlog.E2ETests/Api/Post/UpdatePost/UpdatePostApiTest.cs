using Net.SimpleBlog.Api.ApiModels.Response;
using Net.SimpleBlog.Application.UseCases.Post.Common;
using Net.SimpleBlog.Application.UseCases.Post.UpdatePost;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Net.SimpleBlog.E2ETests.Api.Post.UpdatePost;

[Collection(nameof(UpdatePostApiTestFixture))]
public class UpdatePostApiTest : IDisposable
{
    private readonly UpdatePostApiTestFixture _fixture;

    public UpdatePostApiTest(UpdatePostApiTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(UpdatePost))]
    [Trait("E2E/Api", "Post/Update - Endpoints")]
    public async Task UpdatePost()
    {
        var validUser = _fixture.GetValidUser();
        await _fixture.Persistence.InsertUser(validUser);

        var examplePostsList = _fixture.GetPostsList(validUser.Id, 20);
        await _fixture.Persistence.InsertPosts(examplePostsList);
        var examplePost = examplePostsList[10];
        var postModelInput = _fixture.GetInput(validUser.Id);

        var (response, output) = await _fixture
            .ApiClient
            .Put<ApiResponse<PostModelOutput>>(
            $"/posts/{examplePost.Id}",
            postModelInput
        );

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Title.Should().Be(postModelInput.Title);
        output.Data.Content.Should().Be(postModelInput.Content);
        output.Data.Id.Should().NotBeEmpty();
        output.Data.Id.Should().Be(examplePost.Id);
        output.Data.CreatedAt.Should().NotBeSameDateAs(default);

        var dbPost = await _fixture.Persistence
            .GetPostById(examplePost.Id);
        dbPost.Should().NotBeNull();
        dbPost!.Title.Should().Be(postModelInput.Title);
        dbPost.Content.Should().Be(postModelInput.Content);
        dbPost.Id.Should().NotBeEmpty();
    }

    [Fact(DisplayName = nameof(ErrorWhenNotFound))]
    [Trait("E2E/Api", "Post/Update - Endpoints")]
    public async Task ErrorWhenNotFound()
    {
        var validUser = _fixture.GetValidUser();
        await _fixture.Persistence.InsertUser(validUser);

        var postModelInput = _fixture.GetInput(validUser.Id);

        var (response, output) = await _fixture
            .ApiClient
            .Put<ProblemDetails>($"/posts/{postModelInput.Id}", postModelInput);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
        output.Should().NotBeNull();
        output!.Status.Should().Be((int)StatusCodes.Status404NotFound);
        output.Title.Should().Be("Not found");
        output.Detail.Should().Be($"Post with id {postModelInput.Id} not found");
        output.Type.Should().Be("NotFound");
    }

    [Theory(DisplayName = nameof(ErrorWhenCantInstatiateAggregate))]
    [Trait("E2E/Api", "Post/Update - Endpoints")]
    [MemberData(
        nameof(UpdatePostApiTestDataGenerator.GetInvalidInputs),
        MemberType = typeof(UpdatePostApiTestDataGenerator)
    )]
    public async Task ErrorWhenCantInstatiateAggregate(
        UpdatePostInput input,
        string expectedDetail
    )
    {
        var validUser = _fixture.GetValidUser();
        await _fixture.Persistence.InsertUser(validUser);

        var examplePostsList = _fixture.GetPostsList(validUser.Id, 20);
        await _fixture.Persistence.InsertPosts(examplePostsList);
        var examplePost = examplePostsList[10];

        var (response, output) = await _fixture
            .ApiClient
            .Put<ProblemDetails>($"/posts/{examplePost.Id}", input);

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