using FluentAssertions;
using Moq;
using Net.SimpleBlog.Application.Exceptions;
using Xunit;
using UseCases = Net.SimpleBlog.Application.UseCases.Post.GetPost;

namespace Net.SimpleBlog.UnitTests.Application.Post.GetPost;

[Collection(nameof(GetPostTestFixture))]
public class GetPostTest
{
    private readonly GetPostTestFixture _fixture;

    public GetPostTest(GetPostTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(GetPost))]
    [Trait("Application", "GetPost - Use Cases")]
    public async Task GetPost()
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        var examplePost = _fixture.GetValidPost();
        repositoryMock.Setup(repository => repository.Get(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(examplePost);

        var input = new UseCases.GetPostInput(examplePost.Id);
        var useCase = new UseCases.GetPost(repositoryMock.Object);

        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(repository => repository.Get(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()
        ), Times.Once);

        output.Should().NotBeNull();
        output.Title.Should().Be(examplePost.Title);
        output.Content.Should().Be(examplePost.Content);
        output.UserId.Should().Be(examplePost.UserId);
        output.Id.Should().Be(examplePost.Id);
    }

    [Fact(DisplayName = nameof(NotFoundExceptionWhenPostDoesntExist))]
    [Trait("Application", "GetPost - Use Cases")]
    public async Task NotFoundExceptionWhenPostDoesntExist()
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        var exampleGuid = Guid.NewGuid();
        repositoryMock.Setup(repository => repository.Get(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()
        )).ThrowsAsync(
            new NotFoundException($"Post '{exampleGuid}' not found")
        );

        var input = new UseCases.GetPostInput(exampleGuid);
        var useCase = new UseCases.GetPost(repositoryMock.Object);

        var task = async ()
            => await useCase.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<NotFoundException>();

        repositoryMock.Verify(repository => repository.Get(
            It.IsAny<Guid>(),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }
}
