using Net.SimpleBlog.Application.UseCases.User.Common;
using Net.SimpleBlog.Application.UseCases.User.ListUsers;
using Net.SimpleBlog.Domain.SeedWork.SearchableRepository;
using Net.SimpleBlog.E2ETests.Extensions.DateTime;
using Net.SimpleBlog.E2ETests.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;

namespace Net.SimpleBlog.E2ETests.Api.User.ListUsers;

[Collection(nameof(ListUsersApiTestFixture))]
public class ListUsersApiTest
    : IDisposable
{
    private readonly ListUsersApiTestFixture _fixture;

    public ListUsersApiTest(ListUsersApiTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(ListUserAndTotalByDefault))]
    [Trait("E2E/Api", "User/List - Endpoints")]
    public async Task ListUserAndTotalByDefault()
    {
        var defaultPerPage = 15;
        var exampleUsersList = _fixture.GeUsersList(20);
        await _fixture.Persistence.InsertList(exampleUsersList);

        var (response, output) = await _fixture
            .ApiClient
            .Get<TestApiResponseList<UserModelOutput>>($"/users");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output.Meta.Should().NotBeNull();
        output.Meta.Total.Should().Be(exampleUsersList.Count);
        output.Data.Should().HaveCount(defaultPerPage);
        output!.Data.Should().NotBeNull();
        foreach (UserModelOutput user in output.Data)
        {
            var dbUser = exampleUsersList
                .FirstOrDefault(x => x.Id == user.Id);
            dbUser.Should().NotBeNull();
            dbUser!.Name.Should().Be(user.Name);
            dbUser.Email.Should().Be(user.Email);
            dbUser.Phone.Should().Be(user.Phone);
            dbUser.CPF.Should().Be(user.CPF);
            dbUser.RG.Should().Be(user.RG);
            dbUser.IsActive.Should().Be(user.IsActive);
            dbUser.Id.Should().NotBeEmpty();
        }
    }

    [Fact(DisplayName = nameof(ItemsEmptyWhenPersistenceEmpty))]
    [Trait("E2E/Api", "User/List - Endpoints")]
    public async Task ItemsEmptyWhenPersistenceEmpty()
    {
        var (response, output) = await _fixture
            .ApiClient
            .Get<TestApiResponseList<UserModelOutput>>("/users");

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Data.Should().HaveCount(0);
        output.Meta.Should().NotBeNull();
        output.Meta.Total.Should().Be(0);
        output.Data.Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(ListUsersAndTotal))]
    [Trait("E2E/Api", "User/List - Endpoints")]
    public async Task ListUsersAndTotal()
    {
        var exampleUsersList = _fixture.GeUsersList(20);
        await _fixture.Persistence.InsertList(exampleUsersList);
        var input = new ListUsersInput
        {
            Page = 1,
            PerPage = 5
        };

        var (response, output) = await _fixture
            .ApiClient
            .Get<TestApiResponseList<UserModelOutput>>("/users", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Meta.Total.Should().Be(exampleUsersList.Count);
        foreach (var item in output!.Data)
        {
            var exampleUser = exampleUsersList
                .FirstOrDefault(x => x.Id == item.Id);
            item.Name.Should().Be(exampleUser!.Name);
            item.Email.Should().Be(exampleUser!.Email);
            item.Phone.Should().Be(exampleUser!.Phone);
            item.CPF.Should().Be(exampleUser!.CPF);
            item.RG.Should().Be(exampleUser!.RG);
            item.IsActive.Should().Be(exampleUser!.IsActive);
            item.CreatedAt.TrimMilliSeconds().Should().BeSameDateAs(
                exampleUser!.CreatedAt.TrimMilliSeconds()
            );
        }

    }

    [Theory(DisplayName = "ListPaginated")]
    [Trait("E2E/Api", "User/List - Endpoints")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 3, 5, 0)]
    public async Task ListPaginated(
        int itemsToGenerate,
        int page,
        int perPage,
        int expectedTotal
        )
    {
        var exampleUsersList = _fixture.GeUsersList(itemsToGenerate);
        await _fixture.Persistence.InsertList(exampleUsersList);
        var input = new ListUsersInput
        {
            Page = page,
            PerPage = perPage
        };

        var (response, output) = await _fixture
            .ApiClient
            .Get<TestApiResponseList<UserModelOutput>>("/users", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Meta.Total.Should().Be(exampleUsersList.Count);
        foreach (var item in output!.Data)
        {
            var exampleUser = exampleUsersList
                .FirstOrDefault(x => x.Id == item.Id);
            item.Name.Should().Be(exampleUser!.Name);
            item.Email.Should().Be(exampleUser!.Email);
            item.Phone.Should().Be(exampleUser!.Phone);
            item.CPF.Should().Be(exampleUser!.CPF);
            item.RG.Should().Be(exampleUser!.RG);
            item.IsActive.Should().Be(exampleUser!.IsActive);
            item.CreatedAt.TrimMilliSeconds().Should().BeSameDateAs(
                exampleUser!.CreatedAt.TrimMilliSeconds()
            );
        }

    }

    [Theory(DisplayName = "SearchByText")]
    [Trait("E2E/Api", "User/List - Endpoints")]
    [InlineData("John", 1, 5, 1, 1)]
    [InlineData("Doe", 1, 5, 2, 2)]
    [InlineData("Example", 1, 5, 3, 3)]
    [InlineData("Example", 2, 5, 3, 0)]
    [InlineData("Example", 3, 5, 3, 0)]
    public async Task SearchByText(
        string search,
        int page,
        int perPage,
        int expectedTotalResult,
        int expectedTotalItems
    )
    {
        var exampleUsersList = _fixture.GetExampleUsersListWithNames(
            new List<string>()
            {
                        "Example User 1",
                        "Example User 2",
                        "John Doe",
                        "Jane Doe",
                        "Example User 3",
            }
        );

        await _fixture.Persistence.InsertList(exampleUsersList);
        var input = new ListUsersInput
        {
            Page = page,
            PerPage = perPage,
            Search = search
        };

        var (response, output) = await _fixture
            .ApiClient
            .Get<TestApiResponseList<UserModelOutput>>("/users", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        foreach (var item in output!.Data)
        {
            var exampleUser = exampleUsersList
                .FirstOrDefault(x => x.Id == item.Id);
            item.Name.Should().Be(exampleUser!.Name);
            item.Email.Should().Be(exampleUser!.Email);
            item.Phone.Should().Be(exampleUser!.Phone);
            item.CPF.Should().Be(exampleUser!.CPF);
            item.RG.Should().Be(exampleUser!.RG);
            item.IsActive.Should().Be(exampleUser!.IsActive);
            item.CreatedAt.TrimMilliSeconds().Should().BeSameDateAs(
                exampleUser!.CreatedAt.TrimMilliSeconds()
            );
        }
    }

    [Theory(DisplayName = "SearchOrdered")]
    [Trait("E2E/Api", "User/List - Endpoints")]
    [InlineData("name", "asc")]
    [InlineData("name", "desc")]
    [InlineData("createdAt", "asc")]
    [InlineData("createdAt", "desc")]
    public async Task SearchOrdered(
        string orderBy,
        string order
    )
    {
        var exampleUsersList = _fixture.GeUsersList(10);
        await _fixture.Persistence.InsertList(exampleUsersList);
        var inputOrder = order == "asc"
            ? SearchOrder.Asc
            : SearchOrder.Desc;

        var input = new ListUsersInput
        {
            Page = 1,
            PerPage = 20,
            Search = "",
            Sort = orderBy,
            Dir = inputOrder
        };

        var (response, output) = await _fixture
            .ApiClient
            .Get<TestApiResponseList<UserModelOutput>>("/users", input);

        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Meta.Total.Should().Be(exampleUsersList.Count);
        var expectOrdered = _fixture.SortList(exampleUsersList, input.Sort, input.Dir);

        for (int i = 0; i < output!.Data.Count; i++)
        {
            var outputItem = output.Data[i];
            var exampleItem = expectOrdered[i];
            outputItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem.Name);
            outputItem.Email.Should().Be(exampleItem.Email);
            outputItem.Phone.Should().Be(exampleItem.Phone);
            outputItem.CPF.Should().Be(exampleItem.CPF);
            outputItem.RG.Should().Be(exampleItem.RG);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
        }
    }

    public void Dispose()
    => _fixture.CleanPersistence();
}
