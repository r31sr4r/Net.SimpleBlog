using Bogus.Extensions.Brazil;
using Net.SimpleBlog.Domain.Entity;
using Net.SimpleBlog.Domain.SeedWork.SearchableRepository;
using Net.SimpleBlog.IntegrationTests.Base;

namespace Net.SimpleBlog.IntegrationTests.Infra.Data.EF.Repositories.UserRepository;

[CollectionDefinition(nameof(UserRepositoryTestFixture))]
public class UserRepositoryTestFixtureCollection
    : ICollectionFixture<UserRepositoryTestFixture>
{ }

public class UserRepositoryTestFixture
    : BaseFixture
{
    public string GetValidUserName()
        => Faker.Internet.UserName();


    public string GetValidEmail()
        => Faker.Internet.Email();

    public string GetValidPhone()
    {
        var phoneNumber = Faker.Random.Bool()
            ? Faker.Phone.PhoneNumber("(##) ####-####")
            : Faker.Phone.PhoneNumber("(##) #####-####");

        return phoneNumber;
    }

    public string GetValidCPF()
        => Faker.Person.Cpf();

    public string GetValidRG()
        => Faker.Person.Random.AlphaNumeric(9);

    public DateTime GetValidDateOfBirth()
        => Faker.Person.DateOfBirth;

    public string GetValidPassword()
        => "ValidPassword123!";

    public bool GetRandomBoolean()
    => new Random().NextDouble() < 0.5;

    public User GetExampleUser()
        => new(
            GetValidUserName(),
            GetValidEmail(),
            GetValidPhone(),
            GetValidCPF(),
            GetValidDateOfBirth(),
            GetValidRG(),
            GetValidPassword()
        );

    public List<User> GetExampleUserList(int lenght = 10)
        => Enumerable.Range(1, lenght)
            .Select(_ => GetExampleUser()).ToList();

    public User GetValidUserWithoutPassword()
        => new(
            GetValidUserName(),
            GetValidEmail(),
            GetValidPhone(),
            GetValidCPF(),
            GetValidDateOfBirth(),
            GetValidRG(),
            string.Empty
        );

    public List<User> GetExampleUsersListWithNames(List<string> names)
        => names.Select(name => new User(
            name,
            GetValidEmail(),
            GetValidPhone(),
            GetValidCPF(),
            GetValidDateOfBirth(),
            GetValidRG(),
            GetValidPassword()
        )).ToList();

    public List<User> SortList(
        List<User> usersList,
        string orderBy,
        SearchOrder order
    )
    {
        var listClone = new List<User>(usersList);
        var orderedEnumerable = (orderBy, order) switch
        {
            ("name", SearchOrder.Asc) => listClone.OrderBy(x => x.Name).ToList(),
            ("name", SearchOrder.Desc) => listClone.OrderByDescending(x => x.Name).ToList(),
            ("createdAt", SearchOrder.Asc) => listClone.OrderBy(x => x.CreatedAt).ToList(),
            ("createdAt", SearchOrder.Desc) => listClone.OrderByDescending(x => x.CreatedAt).ToList(),
            _ => listClone.OrderBy(x => x.Name).ToList(),
        };

        return orderedEnumerable.ToList();
    }

}