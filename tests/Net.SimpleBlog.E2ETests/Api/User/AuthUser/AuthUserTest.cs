using Net.SimpleBlog.Api.ApiModels.User;
using Net.SimpleBlog.Application.UseCases.User.AuthUser;
using Net.SimpleBlog.E2ETests.Api.User.AuthUser;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using System.Net;

namespace Net.SimpleBlog.E2ETests.Api.User.AuthUser;
[Collection(nameof(AuthUserApiTestFixture))]
public class AuthUserApiTest
    : IDisposable
{
    private readonly AuthUserApiTestFixture _fixture;

    public AuthUserApiTest(AuthUserApiTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(AuthenticateSuccessfully))]
    [Trait("E2E/Api", "User/Authenticate - Endpoints")]
    public async Task AuthenticateSuccessfully()
    {
        var exampleUsersList = _fixture.GeUsersList(20);
        var createUserInput = _fixture.GetValidUser();
        exampleUsersList.Add(createUserInput);
        await _fixture.Persistence.InsertList(exampleUsersList);

        var authInput = new AuthUserInput(
            createUserInput.Email,
            _fixture.GetValidPassword()
        );

        var (response, authResponse) = await _fixture
            .ApiClient
            .Post<AuthResponse>(
                "/users/authenticate",
                authInput
            );

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.OK);
        authResponse.Should().NotBeNull();
        authResponse!.Email.Should().Be(authInput.Email);
        authResponse.Token.Should().NotBeNullOrWhiteSpace();
    }

    [Fact(DisplayName = nameof(AuthenticateUnsuccessfully))]
    [Trait("E2E/Api", "User/Authenticate - Endpoints")]
    public async Task AuthenticateUnsuccessfully()
    {
        var exampleUsersList = _fixture.GeUsersList(20);
        await _fixture.Persistence.InsertList(exampleUsersList);

        var authInput = new AuthUserInput(
            exampleUsersList[1].Email,
            "wrongpassword"
        );

        var (response, authResponse) = await _fixture
            .ApiClient
            .Post<ProblemDetails>(
                "/users/authenticate",
                authInput
            );

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status401Unauthorized);
        authResponse.Should().NotBeNull();
        authResponse!.Status.Should().Be((int)StatusCodes.Status401Unauthorized);
        authResponse.Title.Should().Be("Authentication error");
        authResponse.Detail.Should().Be($"Invalid email or password.");
        authResponse.Type.Should().Be("Unauthorized");
    }

    public void Dispose()
    => _fixture.CleanPersistence();
}
