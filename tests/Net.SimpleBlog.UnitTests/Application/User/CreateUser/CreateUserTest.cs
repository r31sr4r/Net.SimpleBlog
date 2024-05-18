using FluentAssertions;
using Moq;
using Net.SimpleBlog.Application.UseCases.User.CreateUser;
using Net.SimpleBlog.Domain.Exceptions;
using Xunit;
using UseCases = Net.SimpleBlog.Application.UseCases.User.CreateUser;
using DomainEntity = Net.SimpleBlog.Domain.Entity;

namespace Net.SimpleBlog.UnitTests.Application.User.CreateUser;
[Collection(nameof(CreateUserTestFixture))]
public class CreateUserTest
{
    private readonly CreateUserTestFixture _fixture;

    public CreateUserTest(CreateUserTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(CreateUser))]
    [Trait("Application", "Create User - Use Cases")]
    public async void CreateUser()
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();

        var useCase = new UseCases.CreateUser(
            repositoryMock.Object,
            unitOfWorkMock.Object
        );

        var input = _fixture.GetInput();

        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(
            repository => repository.Insert(
                It.IsAny<DomainEntity.User>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );

        unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.Commit(It.IsAny<CancellationToken>()),
            Times.Once
        );

        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Email.Should().Be(input.Email);
        output.Phone.Should().Be(input.Phone);
        output.CPF.Should().Be(input.CPF);
        output.DateOfBirth.Should().Be(input.DateOfBirth);
        output.RG.Should().Be(input.RG);
        output.IsActive.Should().Be(input.IsActive);
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBeSameDateAs(default);
    }

    [Fact(DisplayName = nameof(CreateUserWithPassword))]
    [Trait("Application", "Create User - Use Cases")]
    public async void CreateUserWithPassword()
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();

        var useCase = new UseCases.CreateUser(
            repositoryMock.Object,
            unitOfWorkMock.Object
        );

        var input = _fixture.GetInputWithPassword();

        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(
            repository => repository.Insert(
                It.IsAny<DomainEntity.User>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );

        unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.Commit(It.IsAny<CancellationToken>()),
            Times.Once
        );

        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Email.Should().Be(input.Email);
        output.Phone.Should().Be(input.Phone);
        output.CPF.Should().Be(input.CPF);
        output.DateOfBirth.Should().Be(input.DateOfBirth);
        output.RG.Should().Be(input.RG);
        output.IsActive.Should().Be(input.IsActive);
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBeSameDateAs(default);
    }

    [Theory(DisplayName = nameof(ThrowWhenCantInstantiateUser))]
    [Trait("Application", "Create User - Use Cases")]
    [MemberData(
    nameof(CreateUserTestDataGenerator.GetInvalidInputs),
    parameters: 12,
    MemberType = typeof(CreateUserTestDataGenerator)
    )]
    public async void ThrowWhenCantInstantiateUser(
    CreateUserInput input,
    string expectionMessage
    )
    {
        var useCase = new UseCases.CreateUser(
            _fixture.GetRepositoryMock().Object,
            _fixture.GetUnitOfWorkMock().Object
        );

        Func<Task> task = async () => await useCase.Handle(input, CancellationToken.None);

        await task.Should()
            .ThrowAsync<EntityValidationException>()
            .WithMessage(expectionMessage);
    }
}
