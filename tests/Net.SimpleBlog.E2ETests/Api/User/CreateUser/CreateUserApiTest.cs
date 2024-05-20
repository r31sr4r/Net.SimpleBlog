using Net.SimpleBlog.Api.ApiModels.Response;
using Net.SimpleBlog.Application.UseCases.User.Common;
using Net.SimpleBlog.Application.UseCases.User.CreateUser;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Net.SimpleBlog.E2ETests.Api.User.CreateUser;

[Collection(nameof(CreateUserApiTestFixture))]
public class CreateUserApiTest
    : IDisposable
{
    private readonly CreateUserApiTestFixture _fixture;

    public CreateUserApiTest(CreateUserApiTestFixture fixture) 
        => _fixture = fixture;

    [Fact(DisplayName = nameof(CreateUser))]
    [Trait("E2E/Api", "User/Create - Endpoints")]
    public async Task CreateUser()
    {
        var input = _fixture.GetInput();

        var (response, output) = await _fixture
            .ApiClient
            .Post<ApiResponse<UserModelOutput>>(
                "/users", 
                input
            );

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be(HttpStatusCode.Created);
        output!.Data.Should().NotBeNull();
        output.Data.Name.Should().Be(input.Name);
        output.Data.Email.Should().Be(input.Email);
        output.Data.Phone.Should().Be(input.Phone);
        output.Data.CPF.Should().Be(input.CPF);
        output.Data.DateOfBirth.Should().Be(input.DateOfBirth);
        output.Data.RG.Should().Be(input.RG);
        output.Data.IsActive.Should().Be(input.IsActive);
        output.Data.Id.Should().NotBeEmpty();
        output.Data.CreatedAt.Should().NotBeSameDateAs(default);

        var dbUser = await _fixture.Persistence
            .GetById(output.Data.Id);
        dbUser.Should().NotBeNull();
        dbUser!.Name.Should().Be(input.Name);
        dbUser.Email.Should().Be(input.Email);
        dbUser.Phone.Should().Be(input.Phone);
        dbUser.CPF.Should().Be(input.CPF);
        dbUser.DateOfBirth.Date.Should().Be(input.DateOfBirth.Date);
        dbUser.RG.Should().Be(input.RG);
        dbUser.IsActive.Should().Be(input.IsActive);
        dbUser.Id.Should().NotBeEmpty();

    }

    [Theory(DisplayName = nameof(ErrorWhenCantInstatiateAggregate))]
    [Trait("E2E/Api", "User/Create - Endpoints")]
    [MemberData(
        nameof(UpdateUserApiTestDataGenerator.GetInvalidInputs),
        MemberType = typeof(UpdateUserApiTestDataGenerator)
    )]
    public async Task ErrorWhenCantInstatiateAggregate(
        CreateUserInput input,
        string expectedDetail
    )
    {
        var (response, output) = await _fixture
            .ApiClient
            .Post<ProblemDetails>(
                "/users",
                input
            );

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
