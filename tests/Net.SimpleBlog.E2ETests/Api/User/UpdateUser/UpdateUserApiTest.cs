using Net.SimpleBlog.Api.ApiModels.Response;
using Net.SimpleBlog.Application.UseCases.User.Common;
using Net.SimpleBlog.Application.UseCases.User.Update;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Net.SimpleBlog.E2ETests.Api.User.UpdateUser;

[Collection(nameof(UpdateUserApiTestFixture))]
public class UpdateUserApiTest
    : IDisposable
{
    private readonly UpdateUserApiTestFixture _fixture;

    public UpdateUserApiTest(UpdateUserApiTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(UpdateUser))]
    [Trait("E2E/Api", "User/Update - Endpoints")]
    public async Task UpdateUser()
    {
        var exampleUsersList = _fixture.GeUsersList(20);
        await _fixture.Persistence.InsertList(exampleUsersList);
        var exampleUser = exampleUsersList[10];
        var userModelInput = _fixture.GetInput(exampleUser.Id);

        var (response, output) = await _fixture
            .ApiClient
            .Put<ApiResponse<UserModelOutput>>(
            $"/users/{exampleUser.Id}",
            userModelInput
        );

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Name.Should().Be(userModelInput.Name);
        output.Data.Email.Should().Be(userModelInput.Email);
        output.Data.Phone.Should().Be(userModelInput.Phone);
        output.Data.CPF.Should().Be(userModelInput.CPF);
        output.Data.RG.Should().Be(userModelInput.RG);
        output.Data.Id.Should().NotBeEmpty();
        output.Data.Id.Should().Be(exampleUser.Id);
        output.Data.CreatedAt.Should().NotBeSameDateAs(default);

        var dbUser = await _fixture.Persistence
            .GetById(exampleUser.Id);
        dbUser.Should().NotBeNull();
        dbUser!.Name.Should().Be(userModelInput.Name);
        dbUser.Email.Should().Be(userModelInput.Email);
        dbUser.Phone.Should().Be(userModelInput.Phone);
        dbUser.CPF.Should().Be(userModelInput.CPF);
        dbUser.RG.Should().Be(userModelInput.RG);
        dbUser.Id.Should().NotBeEmpty();
    }

    [Fact(DisplayName = nameof(ErrorWhenNotFound))]
    [Trait("E2E/Api", "User/Update - Endpoints")]
    public async Task ErrorWhenNotFound()
    {
        var exampleUsersList = _fixture.GeUsersList(20);
        await _fixture.Persistence.InsertList(exampleUsersList);
        var userModelInput = _fixture.GetInput();

        var (response, output) = await _fixture
            .ApiClient
            .Put<ProblemDetails>($"/users/{userModelInput.Id}", userModelInput);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
        output.Should().NotBeNull();
        output!.Status.Should().Be((int)StatusCodes.Status404NotFound);
        output.Title.Should().Be("Not found");
        output.Detail.Should().Be($"User with id {userModelInput.Id} not found");
        output.Type.Should().Be("NotFound");
    }

    [Theory(DisplayName = nameof(ErrorWhenCantInstatiateAggregate))]
    [Trait("E2E/Api", "User/Update - Endpoints")]
    [MemberData(
        nameof(UpdateUserApiTestDataGenerator.GetInvalidInputs),
        MemberType = typeof(UpdateUserApiTestDataGenerator)
    )]
    public async Task ErrorWhenCantInstatiateAggregate(
        UpdateUserInput input,
        string expectedDetail
    )
    {
        var exampleUsersList = _fixture.GeUsersList(20);
        await _fixture.Persistence.InsertList(exampleUsersList);
        var exampleUser = exampleUsersList[10];

        var (response, output) = await _fixture
            .ApiClient
            .Put<ProblemDetails>($"/users/{exampleUser.Id}", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        output.Should().NotBeNull();
        output!.Title.Should().Be("One or more validation errors occurred");
        output.Type.Should().Be("UnprocessableEntity");
        output.Status.Should().Be((int)StatusCodes.Status422UnprocessableEntity);
        output.Detail.Should().Be(expectedDetail);
    }

    public void Dispose()
        => _fixture.CleanPersistence();
}
