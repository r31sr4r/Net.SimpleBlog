using Net.SimpleBlog.Application.UseCases.User.Update;
using Net.SimpleBlog.Infra.Data.EF.Repositories;
using Net.SimpleBlog.Infra.Data.EF;
using DomainEntity = Net.SimpleBlog.Domain.Entity;
using UseCase = Net.SimpleBlog.Application.UseCases.User.Update;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Net.SimpleBlog.Application.Exceptions;
using Net.SimpleBlog.Domain.Exceptions;
using Net.SimpleBlog.Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Net.SimpleBlog.Infra.Data.EF;

namespace Net.SimpleBlog.IntegrationTests.Application.UseCases.User.UpdateUser;

[Collection(nameof(UpdateUserTestFixture))]
public class UpdateUserTest
{
    private readonly UpdateUserTestFixture _fixture;

    public UpdateUserTest(UpdateUserTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Theory(DisplayName = nameof(UpdateUser))]
    [Trait("Integration/Application", "UpdateUser - Use Cases")]
    [MemberData(
        nameof(UpdateUserTestDataGenerator.GetUsersToUpdate),
        parameters: 5,
        MemberType = typeof(UpdateUserTestDataGenerator)
    )]
    public async Task UpdateUser(
        DomainEntity.User userExample,
        UpdateUserInput input
    )
    {
        var dbContext = _fixture.CreateDbContext();
        await dbContext.AddRangeAsync(_fixture.GeUsersList());
        var trackingInfo = await dbContext.AddAsync(userExample);
        dbContext.SaveChanges();
        trackingInfo.State = EntityState.Detached;
        var repository = new UserRepository(dbContext);
        var unitOfWork = new UnitOfWork(
            dbContext
        );
        var useCase = new UseCase.UpdateUser(repository, unitOfWork);

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
        output.DateOfBirth.Date.Should().Be(input.DateOfBirth.Date);
        output.RG.Should().Be(input.RG);
        output.IsActive.Should().Be((bool)input.IsActive!);
    }

    [Fact(DisplayName = nameof(ThrowWhenUserNotFound))]
    [Trait("Integration/Application", "UpdateUser - Use Cases")]
    public async Task ThrowWhenUserNotFound()
    {
        var input = _fixture.GetValidInput();
        var dbContext = _fixture.CreateDbContext();
        await dbContext.AddRangeAsync(_fixture.GeUsersList());
        dbContext.SaveChanges();
        var repository = new UserRepository(dbContext);
        var unitOfWork = new UnitOfWork(
            dbContext
        );
        var useCase = new UseCase.UpdateUser(repository, unitOfWork);

        var task = async ()
            => await useCase.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"User with id {input.Id} not found");
    }

    [Theory(DisplayName = nameof(ThrowWhenCantUpdateUser))]
    [Trait("Integration/Application", "UpdateUser - Use Cases")]
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
        var dbContext = _fixture.CreateDbContext();
        var exampleUsers = _fixture.GeUsersList();
        await dbContext.AddRangeAsync(exampleUsers);
        dbContext.SaveChanges();
        var repository = new UserRepository(dbContext);
        var unitOfWork = new UnitOfWork(
            dbContext
        );
        var useCase = new UseCase.UpdateUser(repository, unitOfWork);
        input.Id = exampleUsers[0].Id;

        var task = async ()
            => await useCase.Handle(input, CancellationToken.None);

        await task.Should()
            .ThrowAsync<EntityValidationException>()
            .WithMessage(expectedMessage); 
    }
}
