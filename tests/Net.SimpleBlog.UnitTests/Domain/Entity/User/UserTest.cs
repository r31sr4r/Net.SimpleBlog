using DomainEntity = Net.SimpleBlog.Domain.Entity;
using Xunit;
using FluentAssertions;
using Net.SimpleBlog.Domain.Exceptions;
using Net.SimpleBlog.Domain.Common.Security;

namespace Net.SimpleBlog.UnitTests.Domain.Entity.User;
public class UserTest
{
    private class PersonData
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? CPF { get; set; }
        public string? RG { get; set; }
        public string? Password { get; set; }
        public DateTime DateOfBirth { get; set; }
    }

    private PersonData GetInitialData() => new PersonData
    {
        Name = "Tenant 1",
        Email = "test@mail.com",
        Phone = "(12) 34567-8910",
        CPF = "07677240038",
        RG = "123456789",
        DateOfBirth = new DateTime(1990, 1, 1),
        Password = "TestPassword123!"
    };

    private DomainEntity.User CreatePerson(PersonData data) =>
        new DomainEntity.User(
            data.Name!,
            data.Email!,
            data.Phone!,
            data.CPF!,
            data.DateOfBirth,
            data.RG,
            data.Password
    );


    [Fact(DisplayName = nameof(Instantiate))]
    [Trait("Domain", "User - Aggregates")]
    public void Instantiate()
    {
        var validData = new
        {
            Name = "Tenant 1",
            Email = "test@mail.com",
            Phone = "(12) 34567-8910",
            CPF = "07677240038",
            RG = "123456789",
            DateOfBirth = new DateTime(1990, 1, 1),
            Password = "TestPassword123!"


        };
        var dateTimeBefore = DateTime.Now;

        var person = new DomainEntity.User(
            validData.Name,
            validData.Email,
            validData.Phone,
            validData.CPF,
            validData.DateOfBirth,
            validData.RG,
            validData.Password
            );
        var dateTimeAfter = DateTime.Now;

        person.Should().NotBeNull();
        person.Name.Should().Be(validData.Name);
        person.Email.Should().Be(validData.Email);
        person.Phone.Should().Be(validData.Phone);
        person.CPF.Should().Be(validData.CPF);
        person.RG.Should().Be(validData.RG);
        person.DateOfBirth.Should().Be(validData.DateOfBirth);
        person.Id.Should().NotBeEmpty();
        person.CreatedAt.Should().NotBe(default);
        person.CreatedAt.Should().BeAfter(dateTimeBefore).And.BeBefore(dateTimeAfter);
        person.IsActive.Should().BeTrue();
    }

    [Theory(DisplayName = nameof(InstantiateWithIsActiveStatus))]
    [Trait("Domain", "User - Aggregates")]
    [InlineData(true)]
    [InlineData(false)]
    public void InstantiateWithIsActiveStatus(bool isActive)
    {
        var validData = new
        {
            Name = "Tenant 1",
            Email = "test@mail.com",
            Phone = "(12) 34567-8910",
            CPF = "07677240038",
            RG = "123456789",
            DateOfBirth = new DateTime(1990, 1, 1),
            Password = "TestPassword123!"
        };
        var dateTimeBefore = DateTime.Now;

        var person = new DomainEntity.User(
            validData.Name,
            validData.Email,
            validData.Phone,
            validData.CPF,
            validData.DateOfBirth,
            validData.RG,
            validData.Password,
            isActive);
        var dateTimeAfter = DateTime.Now;

        person.Should().NotBeNull();
        person.Name.Should().Be(validData.Name);
        person.Email.Should().Be(validData.Email);
        person.Phone.Should().Be(validData.Phone);
        person.CPF.Should().Be(validData.CPF);
        person.RG.Should().Be(validData.RG);
        person.DateOfBirth.Should().Be(validData.DateOfBirth);
        person.Id.Should().NotBeEmpty();
        person.CreatedAt.Should().NotBe(default);
        person.CreatedAt.Should().BeAfter(dateTimeBefore).And.BeBefore(dateTimeAfter);
        person.IsActive.Should().Be(isActive);
    }


    [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsEmpty))]
    [Trait("Domain", "User - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("  ")]
    public void InstantiateErrorWhenNameIsEmpty(string? name)
    {
        var data = new
        {
            Name = name,
            Email = "test@mail.com",
            Phone = "(12) 34567-8910",
            CPF = "07677240038",
            RG = "123456789",
            DateOfBirth = new DateTime(1990, 1, 1),
            Password = "TestPassword123!"
        };

        Action action = () => new DomainEntity.User(
            data.Name!,
            data.Email,
            data.Phone,
            data.CPF,
            data.DateOfBirth,
            data.RG,
            data.Password
            );

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should not be empty or null");
    }

    [Theory(DisplayName = nameof(InstantiateErrorWhenCPFIsEmpty))]
    [Trait("Domain", "User - Aggregates")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("  ")]
    public void InstantiateErrorWhenCPFIsEmpty(string? cpf)
    {
        var data = new
        {
            Name = "Tenant 1",
            Email = "test@mail.com",
            Phone = "(12) 34567-8910",
            CPF = cpf,
            RG = "123456789",
            DateOfBirth = new DateTime(1990, 1, 1),
            Password = "TestPassword123!"
        };

        Action action = () => new DomainEntity.User(
            data.Name,
            data.Email,
            data.Phone,
            data.CPF!,
            data.DateOfBirth,
            data.RG,
            data.Password
            );

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("CPF should not be empty or null");
    }

    [Theory(DisplayName = nameof(InstantiateErrorWhenNameIsLessThan3Characters))]
    [Trait("Domain", "User - Aggregates")]
    [InlineData("ab")]
    [InlineData("a")]
    public void InstantiateErrorWhenNameIsLessThan3Characters(string invalidName)
    {
        Action action =
            () => new DomainEntity.User(
                invalidName,
                "test@mail.com",
                "(12) 34567-8910",
                "07677240038",
                new DateTime(1990, 1, 1),
                "123456789",
                "TestPassword123!"
                );

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should be greater than 3 characters");
    }

    [Fact(DisplayName = nameof(InstantiateErrorWhenNameIsGreaterThan255Characters))]
    [Trait("Domain", "User - Aggregates")]
    public void InstantiateErrorWhenNameIsGreaterThan255Characters()
    {
        var invalidName = new string('a', 256);

        Action action =
                () => new DomainEntity.User(invalidName, "test@mail.com", "(12) 34567-8910", "07677240038", new DateTime(1990, 1, 1), "123456789", "TestPassword123!");

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should be less than 255 characters");
    }

    [Theory(DisplayName = nameof(InstantiateErrorWhenEmailIsInvalid))]
    [Trait("Domain", "User - Aggregates")]
    [InlineData("invalidEmail")]
    [InlineData("invalid.email@.")]
    [InlineData("@invalid.email")]
    public void InstantiateErrorWhenEmailIsInvalid(string invalidEmail)
    {
        Action action =
            () => new DomainEntity.User("Valid Name", invalidEmail, "(12) 34567-8910", "07677240038", new DateTime(1990, 1, 1), "123456789", "TestPassword123!");

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Email is not in a valid format");
    }

    [Theory(DisplayName = nameof(InstantiateErrorWhenPhoneIsInvalid))]
    [Trait("Domain", "User - Aggregates")]
    [InlineData("12345")]
    [InlineData("123456789012345")]
    [InlineData("1234567-89")]
    public void InstantiateErrorWhenPhoneIsInvalid(string invalidPhone)
    {
        Action action =
            () => new DomainEntity.User("Valid Name", "valid@email.com", phone: invalidPhone, "07677240038", new DateTime(1990, 1, 1), "123456789", "TestPassword123!");

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Phone is not in a valid format");
    }

    [Theory(DisplayName = nameof(InstantiateErrorWhenCPFIsInvalid))]
    [Trait("Domain", "User - Aggregates")]
    [InlineData("12345678901")]
    [InlineData("98765432101")]
    public void InstantiateErrorWhenCPFIsInvalid(string invalidCPF)
    {
        Action action =
            () => new DomainEntity.User("Valid Name", "valid@email.com", "(12) 34567-8910", invalidCPF, new DateTime(1990, 1, 1), "123456789", "TestPassword123!");

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("CPF is not valid");
    }

    [Fact(DisplayName = nameof(Activate))]
    [Trait("Domain", "User - Aggregates")]
    public void Activate()
    {
        var validData = new
        {
            Name = "Tenant 1",
            Email = "test@mail.com",
            Phone = "(12) 34567-8910",
            CPF = "07677240038",
            RG = "123456789",
            DateOfBirth = new DateTime(1990, 1, 1),
            Password = "TestPassword123!"
        };

        var person = new DomainEntity.User(
            validData.Name,
            validData.Email,
            validData.Phone,
            validData.CPF,
            validData.DateOfBirth,
            validData.RG,
            validData.Password,
            false);

        person.Activate();

        person.IsActive.Should().BeTrue();
    }

    [Fact(DisplayName = nameof(Deactivate))]
    [Trait("Domain", "User - Aggregates")]
    public void Deactivate()
    {
        var validData = new
        {
            Name = "Tenant 1",
            Email = "test@mail.com",
            Phone = "(12) 34567-8910",
            CPF = "07677240038",
            RG = "123456789",
            DateOfBirth = new DateTime(1990, 1, 1),
            Password = "TestPassword123!"
        };

        var person = new DomainEntity.User(
            validData.Name,
            validData.Email,
            validData.Phone,
            validData.CPF,
            validData.DateOfBirth,
            validData.RG,
            validData.Password,
            true);

        person.Deactivate();

        person.IsActive.Should().BeFalse();
    }

    [Fact(DisplayName = nameof(Update))]
    [Trait("Domain", "User - Aggregates")]
    public void Update()
    {
        var initialData = new
        {
            Name = "Tenant 1",
            Email = "test@mail.com",
            Phone = "(12) 34567-8910",
            CPF = "07677240038",
            RG = "123456789",
            DateOfBirth = new DateTime(1990, 1, 1),
            Password = "TestPassword123!"
        };
        var person = new DomainEntity.User(
            initialData.Name,
            initialData.Email,
            initialData.Phone,
            initialData.CPF,
            initialData.DateOfBirth,
            initialData.RG,
            initialData.Password,
            true
        );
        var updatedData = new
        {
            Name = "Tenant 2",
            Email = "test2@mail.com",
            Phone = "(12) 34567-0000",
            CPF = "22918637033",
            RG = "123456700",
            DateOfBirth = new DateTime(1992, 1, 1)
        };

        person.Update(
            updatedData.Name,
            updatedData.Email,
            updatedData.Phone,
            updatedData.CPF,
            updatedData.DateOfBirth,
            updatedData.RG
        );

        person.Name.Should().Be(updatedData.Name);
        person.Email.Should().Be(updatedData.Email);
        person.Phone.Should().Be(updatedData.Phone);
        person.CPF.Should().Be(updatedData.CPF);
        person.RG.Should().Be(updatedData.RG);
        person.DateOfBirth.Should().Be(updatedData.DateOfBirth);
    }

    [Fact(DisplayName = nameof(UpdateNameOnly))]
    [Trait("Domain", "User - Aggregates")]
    public void UpdateNameOnly()
    {
        var initialData = GetInitialData();
        var person = CreatePerson(initialData);

        var newName = "Updated Name Only";
        person.Update(newName, initialData.Email, initialData.Phone, initialData.CPF, initialData.DateOfBirth, initialData.RG);

        person.Name.Should().Be(newName);
        person.Email.Should().Be(initialData.Email);
        person.Phone.Should().Be(initialData.Phone);
        person.CPF.Should().Be(initialData.CPF);
        person.RG.Should().Be(initialData.RG);
        person.DateOfBirth.Should().Be(initialData.DateOfBirth);
    }

    [Fact(DisplayName = nameof(UpdateEmailOnly))]
    [Trait("Domain", "User - Aggregates")]
    public void UpdateEmailOnly()
    {
        var initialData = GetInitialData();
        var person = CreatePerson(initialData);

        var newEmail = "updatedemail@mail.com";
        person.Update(initialData.Name, newEmail, initialData.Phone, initialData.CPF, initialData.DateOfBirth, initialData.RG);

        person.Email.Should().Be(newEmail);
        person.Name.Should().Be(initialData.Name);
        person.Phone.Should().Be(initialData.Phone);
        person.CPF.Should().Be(initialData.CPF);
        person.RG.Should().Be(initialData.RG);
        person.DateOfBirth.Should().Be(initialData.DateOfBirth);
    }

    [Fact(DisplayName = nameof(UpdateErrorWhenNameIsEmpty))]
    [Trait("Domain", "User - Methods")]
    public void UpdateErrorWhenNameIsEmpty()
    {
        var initialData = GetInitialData();
        var person = CreatePerson(initialData);

        Action action = () => person.Update("", initialData.Email, initialData.Phone, initialData.CPF, initialData.DateOfBirth, initialData.RG);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should not be empty or null");
    }

    [Fact(DisplayName = nameof(UpdateErrorWhenNameIsShorterThan4Characters))]
    [Trait("Domain", "User - Methods")]
    public void UpdateErrorWhenNameIsShorterThan4Characters()
    {
        var initialData = GetInitialData();
        var person = CreatePerson(initialData);

        Action action = () => person.Update("Joh", initialData.Email, initialData.Phone, initialData.CPF, initialData.DateOfBirth, initialData.RG);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should be greater than 3 characters");
    }

    [Fact(DisplayName = nameof(UpdateErrorWhenNameIsGreaterThan255Characters))]
    [Trait("Domain", "User - Methods")]
    public void UpdateErrorWhenNameIsGreaterThan255Characters()
    {
        var invalidName = new string('a', 256);
        var initialData = GetInitialData();
        var person = CreatePerson(initialData);

        Action action = () => person.Update(invalidName, initialData.Email, initialData.Phone, initialData.CPF, initialData.DateOfBirth, initialData.RG);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("Name should be less than 255 characters");
    }

    [Theory(DisplayName = nameof(UpdateErrorWhenCPFIsInvalid))]
    [Trait("Domain", "User - Methods")]
    [InlineData("12345678901")]
    [InlineData("98765432101")]
    [InlineData("00000000000")]
    public void UpdateErrorWhenCPFIsInvalid(string invalidCPF)
    {
        var initialData = GetInitialData();
        var person = CreatePerson(initialData);

        Action action = () => person.Update(initialData.Name, initialData.Email, initialData.Phone, invalidCPF, initialData.DateOfBirth, initialData.RG);

        action.Should()
            .Throw<EntityValidationException>()
            .WithMessage("CPF is not valid");
    }

    [Fact(DisplayName = nameof(UpdatePasswordSuccessfully))]
    [Trait("Domain", "User - Methods")]
    public void UpdatePasswordSuccessfully()
    {
        var user = CreateValidUser();
        var currentPasswordPlainText = "ValidPassword123!";
        var newPassword = "NewSecurePassword123!";

        Action updatePasswordAction = () => user.UpdatePassword(currentPasswordPlainText, newPassword);

        updatePasswordAction.Should().NotThrow();

        PasswordHasher.VerifyPasswordHash(newPassword, user.Password!).Should().BeTrue();
    }


    [Fact(DisplayName = nameof(UpdatePasswordFailsWhenCurrentPasswordIsIncorrect))]
    [Trait("Domain", "User - Methods")]
    public void UpdatePasswordFailsWhenCurrentPasswordIsIncorrect()
    {
        // Arrange
        var user = CreateValidUser();
        var incorrectCurrentPassword = "IncorrectPassword!";
        var newPassword = "NewSecurePassword123!";

        // Act
        Action updatePasswordAction = () => user.UpdatePassword(incorrectCurrentPassword, newPassword);

        // Assert
        updatePasswordAction.Should().Throw<EntityValidationException>()
            .WithMessage("Current password is not valid");
    }

    [Theory(DisplayName = nameof(UpdatePasswordFailsWhenNewPasswordIsInvalid))]
    [Trait("Domain", "User - Methods")]
    [InlineData("short")]
    [InlineData("withoutdigits")]
    [InlineData("WITHOUTLOWERCASE")]
    [InlineData("withoutspecialchar123")]
    public void UpdatePasswordFailsWhenNewPasswordIsInvalid(string invalidNewPassword)
    {
        // Arrange
        var user = CreateValidUser();
        var currentPassword = "ValidPassword123!";

        // Act
        Action updatePasswordAction = () => user.UpdatePassword(currentPassword, invalidNewPassword);

        // Assert
        updatePasswordAction.Should().Throw<EntityValidationException>()
            .WithMessage("New password does not meet complexity requirements");
    }

    private DomainEntity.User CreateValidUser()
    {
        var validData = new
        {
            Name = "John Doe",
            Email = "johndoe@example.com",
            Phone = "(11) 99999-9999",
            CPF = "123.456.789-09",
            DateOfBirth = new DateTime(1980, 1, 1),
            RG = "123456789",
            Password = "ValidPassword123!"
        };

        var user = new DomainEntity.User(
            validData.Name,
            validData.Email,
            validData.Phone,
            validData.CPF,
            validData.DateOfBirth,
            validData.RG,
            validData.Password,
            isActive: true
        );

        return user;
    }

}
