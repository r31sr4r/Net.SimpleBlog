using Net.SimpleBlog.Api.ApiModels.Response;
using Net.SimpleBlog.Application.UseCases.User.Common;
using Net.SimpleBlog.E2ETests.Extensions.DateTime;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Net.SimpleBlog.E2ETests.Api.User.GetUser;

[Collection(nameof(GetUserApiTestFixture))]
public class GetUserApiTest
    : IDisposable
{
    private readonly GetUserApiTestFixture _fixture;

    public GetUserApiTest(GetUserApiTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(GetUser))]
    [Trait("E2E/Api", "User/Get - Endpoints")]
    public async Task GetUser()
    {
        var exampleUsersList = _fixture.GeUsersList(20);
        await _fixture.Persistence.InsertList(exampleUsersList);
        var exampleUser = exampleUsersList[10];

        var (response, output) = await _fixture
            .ApiClient
            .Get<ApiResponse<UserModelOutput>>($"/Users/{exampleUser.Id}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Should().NotBeNull();
        output.Data.Id.Should().Be(exampleUser.Id);
        output.Data.Name.Should().Be(exampleUser.Name);
        output.Data.Email.Should().Be(exampleUser.Email);
        output.Data.Phone.Should().Be(exampleUser.Phone);
        output.Data.CPF.Should().Be(exampleUser.CPF);
        output.Data.RG.Should().Be(exampleUser.RG);
        output.Data.IsActive.Should().Be(exampleUser.IsActive);
        output.Data.CreatedAt.TrimMilliSeconds().Should().BeSameDateAs(
            exampleUser.CreatedAt.TrimMilliSeconds()
        );
    }

    [Fact(DisplayName = nameof(ThrowExceptionWhenNotFound))]
    [Trait("E2E/Api", "User/Get - Endpoints")]
    public async Task ThrowExceptionWhenNotFound()
    {
        var exampleusersList = _fixture.GeUsersList(20);
        await _fixture.Persistence.InsertList(exampleusersList);
        var randomGuid = Guid.NewGuid();

        var (response, output) = await _fixture
            .ApiClient
            .Get<ProblemDetails>($"/users/{randomGuid}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
        output.Should().NotBeNull();
        output!.Status.Should().Be((int)StatusCodes.Status404NotFound);
        output.Title.Should().Be("Not found");
        output.Detail.Should().Be($"User with id {randomGuid} not found");
        output.Type.Should().Be("NotFound");
    }

    public void Dispose()
    => _fixture.CleanPersistence();
}

