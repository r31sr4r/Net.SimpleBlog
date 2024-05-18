using Net.SimpleBlog.Application.Exceptions;
using Net.SimpleBlog.Infra.Data.EF.Repositories;
using FluentAssertions;
using UseCase = Net.SimpleBlog.Application.UseCases.User.GetUser;

namespace Net.SimpleBlog.IntegrationTests.Application.UseCases.User.GetUser;

[Collection(nameof(GetUserTestFixture))]
public class GetUserTest
{
    private readonly GetUserTestFixture _fixture;

    public GetUserTest(GetUserTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(GetUser))]
    [Trait("Integration/Application", "GetUser - Use Cases")]
    public async Task GetUser()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleUser = _fixture.GetValidUser();
        dbContext.Add(exampleUser);
        dbContext.SaveChanges();
        var userRepository = new UserRepository(dbContext);

        var input = new UseCase.GetUserInput(exampleUser.Id);
        var useCase = new UseCase.GetUser(userRepository);

        var output = await useCase.Handle(input, CancellationToken.None);

        var dbUser = await (_fixture.CreateDbContext(true))
            .Users
            .FindAsync(exampleUser.Id);

        dbUser.Should().NotBeNull();
        dbUser!.Name.Should().Be(exampleUser.Name);
        dbUser.Email.Should().Be(exampleUser.Email);
        dbUser.Phone.Should().Be(exampleUser.Phone);
        dbUser.CPF.Should().Be(exampleUser.CPF);
        dbUser.DateOfBirth.Date.Should().Be(exampleUser.DateOfBirth.Date);
        dbUser.RG.Should().Be(exampleUser.RG);
        dbUser.IsActive.Should().Be(exampleUser.IsActive);
        dbUser.Id.Should().Be(exampleUser.Id);

        output.Should().NotBeNull();
        output!.Name.Should().Be(exampleUser.Name);
        output.Email.Should().Be(exampleUser.Email);
        output.Phone.Should().Be(exampleUser.Phone);
        output.CPF.Should().Be(exampleUser.CPF);
        output.DateOfBirth.Date.Should().Be(exampleUser.DateOfBirth.Date);
        output.RG.Should().Be(exampleUser.RG);
        output.IsActive.Should().Be(exampleUser.IsActive);
        output.Id.Should().Be(exampleUser.Id);

    }

    [Fact(DisplayName = nameof(NotFoundExceptionWhenUserDoesntExist))]
    [Trait("Integration/Application", "GetUser - Use Cases")]
    public async Task NotFoundExceptionWhenUserDoesntExist()
    {
        var dbContext = _fixture.CreateDbContext();
        var exampleUser = _fixture.GetValidUser();
        dbContext.Add(exampleUser);
        dbContext.SaveChanges();
        var userRepository = new UserRepository(dbContext);
        var input = new UseCase.GetUserInput(Guid.NewGuid());
        var useCase = new UseCase.GetUser(userRepository);

        var task = async ()
            => await useCase.Handle(input, CancellationToken.None
        );

        await task.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"User with id {input.Id} not found");

    }

}
