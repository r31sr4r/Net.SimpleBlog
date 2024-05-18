using FluentAssertions;
using Moq;
using Net.SimpleBlog.Application.Exceptions;
using Net.SimpleBlog.Application.UseCases.User.Common;
using Net.SimpleBlog.Application.UseCases.User.Update;
using Net.SimpleBlog.Domain.Exceptions;
using DomainEntity = Net.SimpleBlog.Domain.Entity;
using UseCases = Net.SimpleBlog.Application.UseCases.User.Update;
using Xunit;

namespace Net.SimpleBlog.UnitTests.Application.User.UpdateUser;
[Collection(nameof(UpdateUserTestFixture))]
public class UpdateUserTest
{
    private readonly UpdateUserTestFixture _fixture;

    public UpdateUserTest(UpdateUserTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Theory(DisplayName = nameof(UpdateUser))]
    [Trait("Application", "UpdateUser - Use Cases")]
    [MemberData(
        nameof(UpdateUserTestDataGenerator.GetUsersToUpdate),
        parameters: 10,
        MemberType = typeof(UpdateUserTestDataGenerator)
    )]
    public async Task UpdateUser(
        DomainEntity.User categoryExample,
        UpdateUserInput input
    )
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        repositoryMock.Setup(repository => repository.Get(
                        categoryExample.Id,
                        It.IsAny<CancellationToken>())
               ).ReturnsAsync(categoryExample);
        var useCase = new UseCases.UpdateUser
            (repositoryMock.Object,
                       unitOfWorkMock.Object
                              );

        UserModelOutput output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Email.Should().Be(input.Email);
        output.Phone.Should().Be(input.Phone);
        output.CPF.Should().Be(input.CPF);
        output.DateOfBirth.Date.Should().Be(input.DateOfBirth.Date);
        output.RG.Should().Be(input.RG);


        output.IsActive.Should().Be((bool)input.IsActive!);

        repositoryMock.Verify(
            repository => repository.Get(
                categoryExample.Id,
                It.IsAny<CancellationToken>()
                ), Times.Once
        );
        repositoryMock.Verify(
            repository => repository.Update(
                categoryExample,
                It.IsAny<CancellationToken>()
                ), Times.Once
        );
        unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.Commit(
                It.IsAny<CancellationToken>()
                ), Times.Once
         );
    }

    [Theory(DisplayName = nameof(UpdateUserWithoutProvidingIsActive))]
    [Trait("Application", "UpdateUser - Use Cases")]
    [MemberData(
    nameof(UpdateUserTestDataGenerator.GetUsersToUpdate),
    parameters: 10,
    MemberType = typeof(UpdateUserTestDataGenerator)
)]
    public async Task UpdateUserWithoutProvidingIsActive(
    DomainEntity.User categoryExample,
    UpdateUserInput exampleInput
)
    {
        var input = new UpdateUserInput(
            exampleInput.Id,
            exampleInput.Name,
            exampleInput.Email,
            exampleInput.Phone,
            exampleInput.CPF,
            exampleInput.DateOfBirth,
            exampleInput.RG
        );
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        repositoryMock.Setup(repository => repository.Get(
                categoryExample.Id,
                It.IsAny<CancellationToken>())
        ).ReturnsAsync(categoryExample);
        var useCase = new UseCases.UpdateUser
            (repositoryMock.Object,
            unitOfWorkMock.Object);

        UserModelOutput output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Email.Should().Be(input.Email);
        output.Phone.Should().Be(input.Phone);
        output.CPF.Should().Be(input.CPF);
        output.DateOfBirth.Date.Should().Be(input.DateOfBirth.Date);
        output.RG.Should().Be(input.RG);

        output.IsActive.Should().Be((bool)categoryExample.IsActive!);

        repositoryMock.Verify(
            repository => repository.Get(
                categoryExample.Id,
                It.IsAny<CancellationToken>()
                ), Times.Once
        );
        repositoryMock.Verify(
            repository => repository.Update(
                categoryExample,
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
    [Trait("Application", "UpdateUser - Use Cases")]
    public async Task ThrowWhenUserNotFound()
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var input = _fixture.GetValidInput();
        repositoryMock.Setup(repository => repository.Get(
            input.Id,
            It.IsAny<CancellationToken>())
            ).ThrowsAsync(new NotFoundException($"User '{input.Id}' not found"));
        var useCase = new UseCases.UpdateUser(
            repositoryMock.Object,
            unitOfWorkMock.Object
        );

        var task = async ()
            => await useCase.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<NotFoundException>();

        repositoryMock.Verify(
            repository => repository.Get(
                input.Id,
                It.IsAny<CancellationToken>()
                ), Times.Once
        );
    }

    [Theory(DisplayName = nameof(ThrowWhenCantUpdateUser))]
    [Trait("Application", "UpdateUser - Use Cases")]
    [MemberData(
    nameof(UpdateUserTestDataGenerator.GetInvalidInputs),
    parameters: 12,
    MemberType = typeof(UpdateUserTestDataGenerator)
)]
    public async Task ThrowWhenCantUpdateUser(
        UpdateUserInput input,
        string expectedMessage
    )
    {
        var category = _fixture.GetValidUser();
        input.Id = category.Id;
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        repositoryMock.Setup(repository => repository.Get(
                category.Id,
                It.IsAny<CancellationToken>())
        ).ReturnsAsync(category);
        var useCase = new UseCases.UpdateUser
            (repositoryMock.Object,
            unitOfWorkMock.Object);

        var task = async ()
            => await useCase.Handle(input, CancellationToken.None);

        await task.Should()
            .ThrowAsync<EntityValidationException>()
            .WithMessage(expectedMessage);

        repositoryMock.Verify(
            repository => repository.Get(
                category.Id,
                It.IsAny<CancellationToken>()
                ), Times.Once
        );
    }
}
