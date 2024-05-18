using FluentAssertions;
using Moq;
using Net.SimpleBlog.Application.Exceptions;
using Xunit;
using UseCases = Net.SimpleBlog.Application.UseCases.User.DeleteUser;


namespace Net.SimpleBlog.UnitTests.Application.User.DeleteUser;

[Collection(nameof(DeleteUserTestFixture))]
public class DeleteUserTest
{
    private readonly DeleteUserTestFixture _fixture;

    public DeleteUserTest(DeleteUserTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(DeleteUser))]
    [Trait("Application", "DeleteUser - Use Cases")]
    public async Task DeleteUser()
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var userExample = _fixture.GetValidUser();

        repositoryMock.Setup(repository => repository.Get(
                    userExample.Id,
                    It.IsAny<CancellationToken>())
        ).ReturnsAsync(userExample);
        var input = new UseCases.DeleteUserInput(userExample.Id);
        var useCase = new UseCases.DeleteUser
            (repositoryMock.Object,
            unitOfWorkMock.Object
        );

        await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(
            repository => repository.Get(
                userExample.Id,
                It.IsAny<CancellationToken>()
            ), Times.Once
        );

        repositoryMock.Verify(
            repository => repository.Delete(
                userExample,
                It.IsAny<CancellationToken>()
            ), Times.Once
        );

        unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.Commit(
                It.IsAny<CancellationToken>()
            ), Times.Once
        );
    }

    [Fact(DisplayName = nameof(ThrowWhenUserNotFound))]
    [Trait("Application", "DeleteUser - Use Cases")]
    public async Task ThrowWhenUserNotFound()
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var categoryGuid = Guid.NewGuid();
        repositoryMock.Setup(repository => repository.Get(
                    categoryGuid,
                    It.IsAny<CancellationToken>())
        ).ThrowsAsync(
            new NotFoundException($"User '{categoryGuid}' not found")
        );
        var input = new UseCases.DeleteUserInput(categoryGuid);
        var useCase = new UseCases.DeleteUser
            (repositoryMock.Object,
            unitOfWorkMock.Object
        );

        var task = async ()
            => await useCase.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<NotFoundException>();

        repositoryMock.Verify(
            repository => repository.Get(
                categoryGuid,
                It.IsAny<CancellationToken>()
            ), Times.Once
        );
    }
}
