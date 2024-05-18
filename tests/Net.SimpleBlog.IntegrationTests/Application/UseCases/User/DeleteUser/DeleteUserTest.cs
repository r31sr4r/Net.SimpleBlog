using Net.SimpleBlog.Infra.Data.EF.Repositories;
using Net.SimpleBlog.Infra.Data.EF;
using Microsoft.EntityFrameworkCore;
using UseCase = Net.SimpleBlog.Application.UseCases.User.DeleteUser;
using FluentAssertions;
using Net.SimpleBlog.Application.Exceptions;
using Net.SimpleBlog.Application.Interfaces;
using Net.SimpleBlog.Domain.SeedWork;
using Net.SimpleBlog.Application;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Net.SimpleBlog.Infra.Data.EF;

namespace Net.SimpleBlog.IntegrationTests.Application.UseCases.User.DeleteUser;
[Collection(nameof(DeleteUserTestFixture))]
public class DeleteUserTest
{
    private readonly DeleteUserTestFixture _fixture;

    public DeleteUserTest(DeleteUserTestFixture fixture)
    {
        _fixture = fixture;
    }


    [Fact(DisplayName = nameof(DeleteUser))]
    [Trait("Integration/Application", "DeleteUser - Use Cases")]
    public async Task DeleteUser()
    {
        var dbContext = _fixture.CreateDbContext();
        var repository = new UserRepository(dbContext);
        var unitOfWork = new UnitOfWork(
            dbContext
        );
        var userExample = _fixture.GetValidUser();
        await dbContext.AddRangeAsync(_fixture.GeUsersList());
        var tracking = await dbContext.AddAsync(userExample);
        dbContext.SaveChanges();
        tracking.State = EntityState.Detached;
        var useCase = new UseCase.DeleteUser(repository, unitOfWork);
        var input = new UseCase.DeleteUserInput(userExample.Id);

        await useCase.Handle(input, CancellationToken.None);

        var dbUser = await (_fixture.CreateDbContext(true))
            .Users
            .FindAsync(userExample.Id);

        dbUser.Should().BeNull();
    }

    [Fact(DisplayName = nameof(ThrowWhenUserNotFound))]
    [Trait("Integration/Application", "DeleteUser - Use Cases")]
    public async Task ThrowWhenUserNotFound()
    {
        var dbContext = _fixture.CreateDbContext();
        var unitOfWork = new UnitOfWork(
            dbContext
        );
        var exampleUser = _fixture.GetValidUser();
        dbContext.Add(exampleUser);
        dbContext.SaveChanges();
        var userRepository = new UserRepository(dbContext);
        var input = new UseCase.DeleteUserInput(Guid.NewGuid());
        var useCase = new UseCase.DeleteUser(userRepository, unitOfWork);


        var task = async ()
            => await useCase.Handle(input, CancellationToken.None
        );

        await task.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"User with id {input.Id} not found");

    }
}