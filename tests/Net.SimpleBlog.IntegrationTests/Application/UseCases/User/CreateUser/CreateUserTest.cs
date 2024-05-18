using Net.SimpleBlog.Application;
using Net.SimpleBlog.Application.UseCases.User.CreateUser;
using Net.SimpleBlog.Domain.Exceptions;
using Net.SimpleBlog.Infra.Data.EF;
using Net.SimpleBlog.Infra.Data.EF.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UseCase = Net.SimpleBlog.Application.UseCases.User.CreateUser;
using FC.Codeflix.Catalog.Infra.Data.EF;

namespace Net.SimpleBlog.IntegrationTests.Application.UseCases.User.CreateUser;

[Collection(nameof(CreateUserTestFixture))]
public class CreateUserTest
{
    private readonly CreateUserTestFixture _fixture;

    public CreateUserTest(CreateUserTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(CreateUser))]
    [Trait("Integration/Application", "Create User - Use Cases")]
    public async void CreateUser()
    {
        var dbContext = _fixture.CreateDbContext();
        var repository = new UserRepository(dbContext);
        var unitOfWork = new UnitOfWork(
            dbContext
        );        

        var useCase = new UseCase.CreateUser(
            repository, unitOfWork
        );

        var input = _fixture.GetInput();

        var output = await useCase.Handle(input, CancellationToken.None);

        var dbUser = await (_fixture.CreateDbContext(true))
            .Users.FindAsync(output.Id);

        dbUser.Should().NotBeNull();
        dbUser!.Name.Should().Be(input.Name);
        dbUser.Email.Should().Be(input.Email);
        dbUser.Phone.Should().Be(input.Phone);
        dbUser.CPF.Should().Be(input.CPF);
        dbUser.DateOfBirth.Date.Should().Be(input.DateOfBirth.Date);
        dbUser.RG.Should().Be(input.RG);
        dbUser.IsActive.Should().Be(input.IsActive);


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
    [Trait("Integration/Application", "Create User - Use Cases")]
    public async void CreateUserWithPassword()
    {
        var dbContext = _fixture.CreateDbContext();
        var repository = new UserRepository(dbContext);
        var unitOfWork = new UnitOfWork(
            dbContext
        );

        var useCase = new UseCase.CreateUser(
            repository, unitOfWork
        );

        var input = _fixture.GetInputWithPassword();

        var output = await useCase.Handle(input, CancellationToken.None);

        var dbUser = await (_fixture.CreateDbContext(true))
            .Users.FindAsync(output.Id);

        dbUser.Should().NotBeNull();
        dbUser!.Name.Should().Be(input.Name);
        dbUser.Email.Should().Be(input.Email);
        dbUser.Phone.Should().Be(input.Phone);
        dbUser.CPF.Should().Be(input.CPF);
        dbUser.DateOfBirth.Date.Should().Be(input.DateOfBirth.Date);
        dbUser.RG.Should().Be(input.RG);
        dbUser.IsActive.Should().Be(input.IsActive);


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
    [Trait("Integration/Application", "Create User - Use Cases")]
    [MemberData(
        nameof(CreateUserTestDataGenerator.GetInvalidInputs),
        parameters: 6,
        MemberType = typeof(CreateUserTestDataGenerator)
        )]
    public async void ThrowWhenCantInstantiateUser(
        CreateUserInput input,
        string expectionMessage
    )
    {
        var dbContext = _fixture.CreateDbContext();
        var repository = new UserRepository(dbContext);
        var unitOfWork = new UnitOfWork(
            dbContext
        );

        var useCase = new UseCase.CreateUser(
            repository, unitOfWork
        );

        Func<Task> task = async () => await useCase.Handle(input, CancellationToken.None);

        await task.Should()
            .ThrowAsync<EntityValidationException>()
            .WithMessage(expectionMessage);

        var dbUser = _fixture.CreateDbContext(true)
            .Users.AsNoTracking()
            .ToList();

        dbUser.Should().HaveCount(0);
    }

}
