using FluentAssertions;
using Moq;
using Net.SimpleBlog.Application.Exceptions;
using Xunit;
using UseCases = Net.SimpleBlog.Application.UseCases.User.AuthUser;


namespace Net.SimpleBlog.UnitTests.Application.User.AuthUser;
[Collection(nameof(AuthUserTestFixture))]
public class AuthUserTest
{
    private readonly AuthUserTestFixture _fixture;

    public AuthUserTest(AuthUserTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(AuthenticateUserSuccessfully))]
    [Trait("Application", "AuthUser - Use Cases")]
    public async Task AuthenticateUserSuccessfully()
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        var exampleUser = _fixture.GetValidUser();
        repositoryMock.Setup(repository => repository.GetByEmail(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(exampleUser);

        var input = new UseCases.AuthUserInput(exampleUser.Email, "ValidPassword123!");
        var useCase = new UseCases.AuthUser(repositoryMock.Object);

        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(repository => repository.GetByEmail(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()
        ), Times.Once);

        output.Should().NotBeNull();
        output.Name.Should().Be(exampleUser.Name);
        output.Email.Should().Be(exampleUser.Email);
        output.Phone.Should().Be(exampleUser.Phone);
        output.CPF.Should().Be(exampleUser.CPF);
        output.DateOfBirth.Date.Should().Be(exampleUser.DateOfBirth.Date);
        output.RG.Should().Be(exampleUser.RG);
        output.IsActive.Should().Be(exampleUser.IsActive);
    }

    [Fact(DisplayName = nameof(AuthenticationExceptionWhenUserNotFound))]
    [Trait("Application", "AuthUser - Use Cases")]
    public async Task AuthenticationExceptionWhenUserNotFound()
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        repositoryMock.Setup(repository => repository.GetByEmail(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()
        )).ThrowsAsync(
            new CustomAuthenticationException($"Invalid email or password.")
        );

        var input = new UseCases.AuthUserInput("nonexistent@example.com", "Password");
        var useCase = new UseCases.AuthUser(repositoryMock.Object);

        Func<Task> action = async () => await useCase.Handle(input, CancellationToken.None);

        await action.Should().ThrowAsync<CustomAuthenticationException>()
            .WithMessage("Invalid email or password.");

        repositoryMock.Verify(repository => repository.GetByEmail(
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

}