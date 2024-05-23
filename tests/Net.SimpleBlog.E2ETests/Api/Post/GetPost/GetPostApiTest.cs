using Net.SimpleBlog.Api.ApiModels.Response;
using Net.SimpleBlog.Application.UseCases.Post.Common;
using Net.SimpleBlog.E2ETests.Extensions.DateTime;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Net.SimpleBlog.E2ETests.Api.Post.GetPost;

[Collection(nameof(GetPostApiTestFixture))]
public class GetPostApiTest : IDisposable
{
    private readonly GetPostApiTestFixture _fixture;

    public GetPostApiTest(GetPostApiTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(GetPost))]
    [Trait("E2E/Api", "Post/Get - Endpoints")]
    public async Task GetPost()
    {
        // Inserir um usuário válido
        var validUser = _fixture.GetValidUser();
        await _fixture.Persistence.InsertUser(validUser);

        // Inserir posts para o usuário
        var examplePostsList = _fixture.GetPostsList(validUser.Id, 20);
        await _fixture.Persistence.InsertPosts(examplePostsList);
        var examplePost = examplePostsList[10];

        var (response, output) = await _fixture
            .ApiClient
            .Get<ApiResponse<PostModelOutput>>($"/posts/{examplePost.Id}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data.Id.Should().Be(examplePost.Id);
        output.Data.Title.Should().Be(examplePost.Title);
        output.Data.Content.Should().Be(examplePost.Content);
        output.Data.UserId.Should().Be(examplePost.UserId);
        output.Data.CreatedAt.TrimMilliSeconds().Should().BeSameDateAs(
            examplePost.CreatedAt.TrimMilliSeconds()
        );
    }

    [Fact(DisplayName = nameof(ThrowExceptionWhenNotFound))]
    [Trait("E2E/Api", "Post/Get - Endpoints")]
    public async Task ThrowExceptionWhenNotFound()
    {
        var validUser = _fixture.GetValidUser();
        await _fixture.Persistence.InsertUser(validUser);

        var examplePostsList = _fixture.GetPostsList(validUser.Id, 20);
        await _fixture.Persistence.InsertPosts(examplePostsList);
        var randomGuid = Guid.NewGuid();

        var (response, output) = await _fixture
            .ApiClient
            .Get<ProblemDetails>($"/posts/{randomGuid}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
        output.Should().NotBeNull();
        output!.Status.Should().Be((int)StatusCodes.Status404NotFound);
        output.Title.Should().Be("Not found");
        output.Detail.Should().Be($"Post with id {randomGuid} not found");
        output.Type.Should().Be("NotFound");
    }

    public void Dispose()
        => _fixture.CleanPersistence();
}
