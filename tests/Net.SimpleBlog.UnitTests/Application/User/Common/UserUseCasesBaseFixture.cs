using Bogus;
using Bogus.Extensions.Brazil;
using Moq;
using Net.SimpleBlog.Application.Interfaces;
using Net.SimpleBlog.Domain.Repository;
using Net.SimpleBlog.UnitTests.Common;
using DomainEntity = Net.SimpleBlog.Domain.Entity;


namespace Net.SimpleBlog.UnitTests.Application.User.Common;
public class UserUseCasesBaseFixture
    : BaseFixture
{
    public Mock<IUserRepository> GetRepositoryMock() => new();

    public Mock<IUnitOfWork> GetUnitOfWorkMock() => new();

    public string GetValidUserName()
    {
        var userName = "";
        while (userName.Length < 3)
            userName = Faker.Person.FullName;
        if (userName.Length > 255)
            userName = userName[..255];
        return userName;
    }

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

    public DomainEntity.User GetValidUser()
        => new(
            GetValidUserName(),
            GetValidEmail(),
            GetValidPhone(),
            GetValidCPF(),
            GetValidDateOfBirth(),
            GetValidRG(),
            GetValidPassword()
        );

    public DomainEntity.User GetValidUserWithoutPassword()
        => new(
            GetValidUserName(),
            GetValidEmail(),
            GetValidPhone(),
            GetValidCPF(),
            GetValidDateOfBirth(),
            GetValidRG(),
            string.Empty
        );

    public List<DomainEntity.User> GeUsersList(int lenght = 10)
    {
        return Enumerable.Range(1, lenght)
            .Select(_ => GetValidUser()
        ).ToList();
    }
}
