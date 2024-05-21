using FluentAssertions;
using Moq;
using Net.SimpleBlog.Application.Exceptions;
using Xunit;
using UseCases = Net.SimpleBlog.Application.UseCases.Post.DeletePost;

namespace Net.SimpleBlog.UnitTests.Application.Post.DeletePost;

[Collection(nameof(DeletePostTestFixture))]
public class DeletePostTest
{
    private readonly DeletePostTestFixture _fixture;

    public DeletePostTest(DeletePostTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(DeletePost))]
    [Trait("Application", "DeletePost - Use Cases")]
    public async Task DeletePost()
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var postExample = _fixture.GetValidPost();

        repositoryMock.Setup(repository => repository.Get(
            postExample.Id,
            It.IsAny<CancellationToken>())
        ).ReturnsAsync(postExample);

        var input = new UseCases.DeletePostInput(postExample.Id);
        var useCase = new UseCases.DeletePost(
            repositoryMock.Object,
            unitOfWorkMock.Object
        );

        await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(
            repository => repository.Get(
                postExample.Id,
                It.IsAny<CancellationToken>()
            ), Times.Once
        );

        repositoryMock.Verify(
            repository => repository.Delete(
                postExample,
                It.IsAny<CancellationToken>()
            ), Times.Once
        );

        unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.Commit(
                It.IsAny<CancellationToken>()
            ), Times.Once
        );
    }

    [Fact(DisplayName = nameof(ThrowWhenPostNotFound))]
    [Trait("Application", "DeletePost - Use Cases")]
    public async Task ThrowWhenPostNotFound()
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var postGuid = Guid.NewGuid();

        repositoryMock.Setup(repository => repository.Get(
            postGuid,
            It.IsAny<CancellationToken>())
        ).ThrowsAsync(
            new NotFoundException($"Post '{postGuid}' not found")
        );

        var input = new UseCases.DeletePostInput(postGuid);
        var useCase = new UseCases.DeletePost(
            repositoryMock.Object,
            unitOfWorkMock.Object
        );

        var task = async ()
            => await useCase.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<NotFoundException>();

        repositoryMock.Verify(
            repository => repository.Get(
                postGuid,
                It.IsAny<CancellationToken>()
            ), Times.Once
        );
    }
}
