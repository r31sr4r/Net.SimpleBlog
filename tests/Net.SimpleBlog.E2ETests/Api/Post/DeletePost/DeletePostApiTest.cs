using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Net.SimpleBlog.E2ETests.Api.Post.Common;
using System.Net;
using Xunit;

namespace Net.SimpleBlog.E2ETests.Api.Post.DeletePost;

[Collection(nameof(PostBaseFixture))]
public class DeletePostApiTest : IAsyncLifetime, IDisposable
{
    private readonly PostBaseFixture _fixture;

    public DeletePostApiTest(PostBaseFixture fixture)
        => _fixture = fixture;

    public async Task InitializeAsync()
    {
        await _fixture.Authenticate();
    }

    public Task DisposeAsync()
    {
        _fixture.CleanPersistence();
        return Task.CompletedTask;
    }

    [Fact(DisplayName = nameof(DeletePost))]
    [Trait("E2E/Api", "Post/Delete - Endpoints")]
    public async Task DeletePost()
    {
        var examplePostsList = _fixture.GetPostsList(_fixture.AuthenticatedUser.Id, 20);
        await _fixture.Persistence.InsertPosts(examplePostsList);
        var examplePost = examplePostsList[10];

        var (response, output) = await _fixture
            .ApiClient
            .Delete<object>($"/posts/{examplePost.Id}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status204NoContent);
        output.Should().BeNull();

        var post = await _fixture.Persistence
            .GetPostById(examplePost.Id);
        post.Should().BeNull();
    }

    [Fact(DisplayName = nameof(ErrorWhenNotFound))]
    [Trait("E2E/Api", "Post/Delete - Endpoints")]
    public async Task ErrorWhenNotFound()
    {
        var examplePostsList = _fixture.GetPostsList(_fixture.AuthenticatedUser.Id, 20);
        await _fixture.Persistence.InsertPosts(examplePostsList);
        var randomGuid = Guid.NewGuid();

        var (response, output) = await _fixture
            .ApiClient
            .Delete<ProblemDetails>($"/posts/{randomGuid}");

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
