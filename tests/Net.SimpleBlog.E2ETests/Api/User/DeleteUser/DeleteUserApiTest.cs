using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Net.SimpleBlog.E2ETests.Api.User.DeleteUser;

[Collection(nameof(DeleteUserApiTestFixture))]
public class DeleteUserApiTest
    : IDisposable
{
    private readonly DeleteUserApiTestFixture _fixture;

    public DeleteUserApiTest(DeleteUserApiTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(DeleteUser))]
    [Trait("E2E/Api", "User/Delete - Endpoints")]
    public async Task DeleteUser()
    {
        var exampleUsersList = _fixture.GeUsersList(20);
        await _fixture.Persistence.InsertList(exampleUsersList);
        var exampleUser = exampleUsersList[10];

        var (response, output) = await _fixture
            .ApiClient
            .Delete<object>($"/users/{exampleUser.Id}");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status204NoContent);
        output.Should().BeNull();

        var user = await _fixture.Persistence
            .GetById(exampleUser.Id);
        user.Should().BeNull();
    }

    [Fact(DisplayName = nameof(ErrorWhenNotFound))]
    [Trait("E2E/Api", "User/Delete - Endpoints")]
    public async Task ErrorWhenNotFound()
    {
        var exampleCategoriesList = _fixture.GeUsersList(20);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var randomGuid = Guid.NewGuid();

        var (response, output) = await _fixture
            .ApiClient
            .Delete<ProblemDetails>($"/users/{randomGuid}");

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
