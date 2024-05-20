using Net.SimpleBlog.Infra.Data.EF;
using Net.SimpleBlog.Infra.Data.EF.Repositories;
using Net.SimpleBlog.Application.UseCases.User.ListUsers;
using UseCase = Net.SimpleBlog.Application.UseCases.User.ListUsers;
using FluentAssertions;
using Net.SimpleBlog.Application.UseCases.User.Common;
using Net.SimpleBlog.Domain.SeedWork.SearchableRepository;

namespace Net.SimpleBlog.IntegrationTests.Application.UseCases.User.ListUser;

[Collection(nameof(ListUsersTestFixture))]
public class ListUsersTest
{
    private readonly ListUsersTestFixture _fixture;

    public ListUsersTest(ListUsersTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = "SearchReturnsListAndTotal")]
    [Trait("Integration/Application", "LiseUsers - Use Cases")]
    public async Task SearchReturnsListAndTotal()
    {
        NetSimpleBlogDbContext dbContext = _fixture.CreateDbContext();
        var exampleUserList = _fixture.GeUsersList(15);
        await dbContext.AddRangeAsync(exampleUserList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var userRepository = new UserRepository(dbContext);
        var searchInput = new ListUsersInput(page: 1, perPage: 10);
        var useCase = new UseCase.ListUsers(userRepository);

        var output = await useCase.Handle(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.Page.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(exampleUserList.Count);
        output.Items.Should().HaveCount(10);
        foreach (UserModelOutput outputItem in output.Items)
        {
            var exampleItem = exampleUserList.Find(x => x.Id == outputItem.Id);
            outputItem.Should().NotBeNull();
            outputItem!.Id.Should().Be(exampleItem!.Id);
            outputItem.Name.Should().Be(exampleItem.Name);
            outputItem.Email.Should().Be(exampleItem.Email);
            outputItem.Phone.Should().Be(exampleItem.Phone);
            outputItem.CPF.Should().Be(exampleItem.CPF);
            outputItem.DateOfBirth.Date.Should().Be(exampleItem.DateOfBirth.Date);
            outputItem.RG.Should().Be(exampleItem.RG);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
        }
    }

    [Fact(DisplayName = "SearchReturnsEmpty")]
    [Trait("Integration/Application", "LiseUsers - Use Cases")]
    public async Task SearchReturnsEmpty()
    {
        NetSimpleBlogDbContext dbContext = _fixture.CreateDbContext();
        var userRepository = new UserRepository(dbContext);
        var searchInput = new ListUsersInput(page: 1, perPage: 10);
        var useCase = new UseCase.ListUsers(userRepository);

        var output = await useCase.Handle(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.Page.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(0);
        output.Items.Should().HaveCount(0);
    }

    [Theory(DisplayName = "SearchReturnsPaginated")]
    [Trait("Integration/Application", "LiseUsers - Use Cases")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 3, 5, 0)]
    public async Task SearchReturnsPaginated(
    int itemsToGenerate,
    int page,
    int perPage,
    int expectedTotal
)
    {
        NetSimpleBlogDbContext dbContext = _fixture.CreateDbContext();
        var exampleUserList = _fixture.GeUsersList(itemsToGenerate);
        await dbContext.AddRangeAsync(exampleUserList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var userRepository = new UserRepository(dbContext);
        var searchInput = new ListUsersInput(page, perPage);
        var useCase = new UseCase.ListUsers(userRepository);

        var output = await useCase.Handle(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.Page.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(exampleUserList.Count);
        output.Items.Should().HaveCount(expectedTotal);
        foreach (UserModelOutput outputItem in output.Items)
        {
            var exampleItem = exampleUserList.Find(x => x.Id == outputItem.Id);
            outputItem.Should().NotBeNull();
            outputItem!.Id.Should().Be(exampleItem!.Id);
            outputItem.Name.Should().Be(exampleItem.Name);
            outputItem.Email.Should().Be(exampleItem.Email);
            outputItem.Phone.Should().Be(exampleItem.Phone);
            outputItem.CPF.Should().Be(exampleItem.CPF);
            outputItem.DateOfBirth.Date.Should().Be(exampleItem.DateOfBirth.Date);
            outputItem.RG.Should().Be(exampleItem.RG);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);

        }
    }

    [Theory(DisplayName = "SearchByText")]
    [Trait("Integration/Application", "LiseUsers - Use Cases")]
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
        NetSimpleBlogDbContext dbContext = _fixture.CreateDbContext();
        var exampleUserList = _fixture.GetExampleUsersListWithNames(
            new List<string>()
            {
                "Example User 1",
                "Example User 2",
                "John Doe",
                "Jane Doe",
                "Example User 3",
            }
        );
        await dbContext.AddRangeAsync(exampleUserList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var userRepository = new UserRepository(dbContext);
        var searchInput = new ListUsersInput(page, perPage, search);
        var useCase = new UseCase.ListUsers(userRepository);

        var output = await useCase.Handle(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.Page.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(expectedTotalResult);
        output.Items.Should().HaveCount(expectedTotalItems);
        foreach (UserModelOutput outputItem in output.Items)
        {
            var exampleItem = exampleUserList.Find(x => x.Id == outputItem.Id);
            outputItem.Should().NotBeNull();
            outputItem!.Id.Should().Be(exampleItem!.Id);
            outputItem.Name.Should().Be(exampleItem.Name);
            outputItem.Email.Should().Be(exampleItem.Email);
            outputItem.Phone.Should().Be(exampleItem.Phone);
            outputItem.CPF.Should().Be(exampleItem.CPF);
            outputItem.DateOfBirth.Date.Should().Be(exampleItem.DateOfBirth.Date);
            outputItem.RG.Should().Be(exampleItem.RG);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);

        }
    }

    [Theory(DisplayName = "SearchOrdered")]
    [Trait("Integration/Application", "LiseUsers - Use Cases")]
    [InlineData("name", "asc")]
    [InlineData("name", "desc")]
    [InlineData("createdAt", "asc")]
    [InlineData("createdAt", "desc")]
    public async Task SearchOrdered(
        string orderBy,
        string order
    )
    {
        NetSimpleBlogDbContext dbContext = _fixture.CreateDbContext();
        var exampleUserList = _fixture.GeUsersList(10);
        await dbContext.AddRangeAsync(exampleUserList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var userRepository = new UserRepository(dbContext);
        var searchOrder = order == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
        var searchInput = new ListUsersInput(
            page: 1, 
            perPage: 20, 
            "",
            orderBy,
            searchOrder
        );
        var useCase = new UseCase.ListUsers(userRepository);

        var output = await useCase.Handle(searchInput, CancellationToken.None);

        var expectOrdered = _fixture.SortList(exampleUserList, orderBy, searchOrder);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.Page.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(exampleUserList.Count);
        output.Items.Should().HaveCount(exampleUserList.Count);

        for (int i = 0; i < output.Items.Count; i++)
        {
            var outputItem = output.Items[i];
            var exampleItem = expectOrdered[i];
            outputItem.Should().NotBeNull();
            outputItem!.Id.Should().Be(exampleItem.Id);
            outputItem.Name.Should().Be(exampleItem.Name);
            outputItem.Email.Should().Be(exampleItem.Email);
            outputItem.Phone.Should().Be(exampleItem.Phone);
            outputItem.CPF.Should().Be(exampleItem.CPF);
            outputItem.DateOfBirth.Date.Should().Be(exampleItem.DateOfBirth.Date);
            outputItem.RG.Should().Be(exampleItem.RG);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
        }

    }
}