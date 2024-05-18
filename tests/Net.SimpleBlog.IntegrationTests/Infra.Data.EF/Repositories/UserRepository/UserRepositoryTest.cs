using FluentAssertions;
using Net.SimpleBlog.Application.Exceptions;
using Net.SimpleBlog.Domain.Entity;
using Net.SimpleBlog.Domain.SeedWork.SearchableRepository;
using Net.SimpleBlog.Infra.Data.EF;
using Repository = Net.SimpleBlog.Infra.Data.EF.Repositories;

namespace Net.SimpleBlog.IntegrationTests.Infra.Data.EF.Repositories.UserRepository;
[Collection(nameof(UserRepositoryTestFixture))]
public class UserRepositoryTest
{
    private readonly UserRepositoryTestFixture _fixture;

    public UserRepositoryTest(UserRepositoryTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = "Insert")]
    [Trait("Integration/Infra.Data", "UserRepository - Repositories")]
    public async Task Insert()
    {
        NetSimpleBlogDbContext dbContext = _fixture.CreateDbContext();
        var exampleUser = _fixture.GetExampleUser();
        var userRepository = new Repository.UserRepository(dbContext);

        await userRepository.Insert(exampleUser, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var dbUser = await (_fixture.CreateDbContext(true))
            .Users.FindAsync(exampleUser.Id);

        dbUser.Should().NotBeNull();
        dbUser.Id.Should().Be(exampleUser.Id);
        dbUser.Name.Should().Be(exampleUser.Name);
        dbUser.Email.Should().Be(exampleUser.Email);
        dbUser.Phone.Should().Be(exampleUser.Phone);
        dbUser.CPF.Should().Be(exampleUser.CPF);
        dbUser.DateOfBirth.Date.Should().Be(exampleUser.DateOfBirth.Date);
        dbUser.RG.Should().Be(exampleUser.RG);
        dbUser.IsActive.Should().Be(exampleUser.IsActive);
    }

    [Fact(DisplayName = "Get")]
    [Trait("Integration/Infra.Data", "UserRepository - Repositories")]
    public async Task Get()
    {
        NetSimpleBlogDbContext dbContext = _fixture.CreateDbContext();
        var exampleUser = _fixture.GetExampleUser();
        var exampleUserList = _fixture.GetExampleUserList(15);
        exampleUserList.Add(exampleUser);
        await dbContext.AddRangeAsync(exampleUserList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var userRepository = new Repository.UserRepository(_fixture.CreateDbContext(true));

        var dbUser = await userRepository.Get(exampleUser.Id, CancellationToken.None);

        dbUser.Should().NotBeNull();
        dbUser.Id.Should().Be(exampleUser.Id);
        dbUser.Name.Should().Be(exampleUser.Name);
        dbUser.Email.Should().Be(exampleUser.Email);
        dbUser.Phone.Should().Be(exampleUser.Phone);
        dbUser.CPF.Should().Be(exampleUser.CPF);
        dbUser.DateOfBirth.Date.Should().Be(exampleUser.DateOfBirth.Date);
        dbUser.RG.Should().Be(exampleUser.RG);
        dbUser.IsActive.Should().Be(exampleUser.IsActive);
    }

    [Fact(DisplayName = "GetThrowIfNotFound")]
    [Trait("Integration/Infra.Data", "UserRepository - Repositories")]
    public async Task GetThrowIfNotFound()
    {
        NetSimpleBlogDbContext dbContext = _fixture.CreateDbContext();
        var exampleId = Guid.NewGuid();
        var exampleUserList = _fixture.GetExampleUserList(15);
        await dbContext.AddRangeAsync(exampleUserList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var userRepository = new Repository.UserRepository(dbContext);

        var task = async () => await userRepository.Get(exampleId, CancellationToken.None);

        await task.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"User with id {exampleId} not found");
    }

    [Fact(DisplayName = "Update")]
    [Trait("Integration/Infra.Data", "UserRepository - Repositories")]
    public async Task Update()
    {
        NetSimpleBlogDbContext dbContext = _fixture.CreateDbContext();
        var exampleUser = _fixture.GetExampleUser();
        var newUser = _fixture.GetExampleUser();
        var exampleUserList = _fixture.GetExampleUserList(15);
        exampleUserList.Add(exampleUser);
        await dbContext.AddRangeAsync(exampleUserList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var userRepository = new Repository.UserRepository(dbContext);

        exampleUser.Update(newUser.Name, newUser.Email, newUser.Phone, newUser.CPF, newUser.DateOfBirth, newUser.RG);
        await userRepository.Update(exampleUser, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var dbUser = await (_fixture.CreateDbContext(true))
            .Users.FindAsync(exampleUser.Id);

        dbUser.Should().NotBeNull();
        dbUser!.Id.Should().Be(exampleUser.Id);
        dbUser.Name.Should().Be(exampleUser.Name);
        dbUser.Email.Should().Be(exampleUser.Email);
        dbUser.Phone.Should().Be(exampleUser.Phone);
        dbUser.CPF.Should().Be(exampleUser.CPF);
        dbUser.DateOfBirth.Date.Should().Be(exampleUser.DateOfBirth.Date);
        dbUser.RG.Should().Be(exampleUser.RG);
        dbUser.IsActive.Should().Be(exampleUser.IsActive);
    }

    [Fact(DisplayName = "Delete")]
    [Trait("Integration/Infra.Data", "UserRepository - Repositories")]
    public async Task Delete()
    {
        NetSimpleBlogDbContext dbContext = _fixture.CreateDbContext();
        var exampleUser = _fixture.GetExampleUser();
        var exampleUserList = _fixture.GetExampleUserList(15);
        exampleUserList.Add(exampleUser);
        await dbContext.AddRangeAsync(exampleUserList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var userRepository = new Repository.UserRepository(dbContext);

        await userRepository.Delete(exampleUser, CancellationToken.None);
        await dbContext.SaveChangesAsync(CancellationToken.None);

        var dbUser = await (_fixture.CreateDbContext(true))
            .Users.FindAsync(exampleUser.Id);

        dbUser.Should().BeNull();
    }

    [Fact(DisplayName = "SearchReturnsListAndTotal")]
    [Trait("Integration/Infra.Data", "UserRepository - Repositories")]
    public async Task SearchReturnsListAndTotal()
    {
        NetSimpleBlogDbContext dbContext = _fixture.CreateDbContext();
        var exampleUserList = _fixture.GetExampleUserList(15);
        await dbContext.AddRangeAsync(exampleUserList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var userRepository = new Repository.UserRepository(dbContext);
        var searchInput = new SearchInput(
            page: 1,
            perPage: 10,
            search: "",
            orderBy: "",
            SearchOrder.Asc
        );

        var output = await userRepository.Search(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(exampleUserList.Count);
        output.Items.Should().HaveCount(10);
        foreach (User outputItem in output.Items)
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
    [Trait("Integration/Infra.Data", "UserRepository - Repositories")]
    public async Task SearchReturnsEmpty()
    {
        NetSimpleBlogDbContext dbContext = _fixture.CreateDbContext();
        var userRepository = new Repository.UserRepository(dbContext);
        var searchInput = new SearchInput(
            page: 1,
            perPage: 10,
            search: "",
            orderBy: "",
            SearchOrder.Asc
        );

        var output = await userRepository.Search(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(0);
        output.Items.Should().HaveCount(0);
    }


    [Theory(DisplayName = "SearchReturnsPaginated")]
    [Trait("Integration/Infra.Data", "UserRepository - Repositories")]
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
        var exampleUserList = _fixture.GetExampleUserList(itemsToGenerate);
        await dbContext.AddRangeAsync(exampleUserList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var userRepository = new Repository.UserRepository(dbContext);
        var searchInput = new SearchInput(
            page: page,
            perPage: perPage,
            search: "",
            orderBy: "",
            SearchOrder.Asc
        );

        var output = await userRepository.Search(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(exampleUserList.Count);
        output.Items.Should().HaveCount(expectedTotal);
        foreach (User outputItem in output.Items)
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
    [Trait("Integration/Infra.Data", "UserRepository - Repositories")]
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
        var userRepository = new Repository.UserRepository(dbContext);
        var searchInput = new SearchInput(
            page,
            perPage,
            search,
            orderBy: "",
            SearchOrder.Asc
        );

        var output = await userRepository.Search(searchInput, CancellationToken.None);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
        output.PerPage.Should().Be(searchInput.PerPage);
        output.Total.Should().Be(expectedTotalResult);
        output.Items.Should().HaveCount(expectedTotalItems);
        foreach (User outputItem in output.Items)
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
    [Trait("Integration/Infra.Data", "UserRepository - Repositories")]
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
        var exampleUserList = _fixture.GetExampleUserList(10);
        await dbContext.AddRangeAsync(exampleUserList);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var userRepository = new Repository.UserRepository(dbContext);
        var searchOrder = order == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
        var searchInput = new SearchInput(
            page: 1,
            perPage: 20,
            search: "",
            orderBy,
            searchOrder
        );

        var output = await userRepository.Search(searchInput, CancellationToken.None);

        var expectOrdered = _fixture.SortList(exampleUserList, orderBy, searchOrder);

        output.Should().NotBeNull();
        output.Items.Should().NotBeNull();
        output.CurrentPage.Should().Be(searchInput.Page);
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

    [Fact(DisplayName = "GetByEmail_ReturnsUser_WhenUserExists")]
    [Trait("Integration/Infra.Data", "UserRepository - Repositories")]
    public async Task GetByEmail_ReturnsUser_WhenUserExists()
    {
        NetSimpleBlogDbContext dbContext = _fixture.CreateDbContext();
        var exampleUser = _fixture.GetExampleUser();
        await dbContext.Users.AddAsync(exampleUser);
        await dbContext.SaveChangesAsync(CancellationToken.None);
        var userRepository = new Repository.UserRepository(dbContext);

        var result = await userRepository.GetByEmail(exampleUser.Email, CancellationToken.None);

        result.Should().NotBeNull();
        result.Email.Should().Be(exampleUser.Email);
    }

    [Fact(DisplayName = "GetByEmail_ThrowsNotFoundException_WhenUserDoesNotExist")]
    [Trait("Integration/Infra.Data", "UserRepository - Repositories")]
    public async Task GetByEmail_ThrowsNotFoundException_WhenUserDoesNotExist()
    {
        NetSimpleBlogDbContext dbContext = _fixture.CreateDbContext();
        var userRepository = new Repository.UserRepository(dbContext);
        var nonExistingEmail = "nonexisting@example.com";

        var task = async () => await userRepository.GetByEmail(nonExistingEmail, CancellationToken.None);
        await task.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"User with email {nonExistingEmail} not found");
    }


}