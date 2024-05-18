using Net.SimpleBlog.Application.UseCases.User.CreateUser;
using Net.SimpleBlog.IntegrationTests.Application.UseCases.User.Common;

namespace Net.SimpleBlog.IntegrationTests.Application.UseCases.User.CreateUser;

[CollectionDefinition(nameof(CreateUserTestFixture))]
public class CreateUserTestFixtureCollection
    : ICollectionFixture<CreateUserTestFixture>
{ }

public class CreateUserTestFixture
    : UserUseCasesBaseFixture
{
    public CreateUserInput GetInput()
    {
        var user = GetValidUserWithoutPassword();
        return new CreateUserInput(
            user.Name,
            user.Email,
            user.Phone,
            user.CPF,
            user.DateOfBirth,
            user.RG,
            user.Password
        );
    }

    public CreateUserInput GetInputWithPassword()
    {
        var user = GetValidUser();
        return new CreateUserInput(
            user.Name,
            user.Email,
            user.Phone,
            user.CPF,
            user.DateOfBirth,
            user.RG,
            user.Password
        );
    }


    public CreateUserInput GetInputWithInvalidEmail()
    {
        var user = GetInput();
        user.Email = "invalid-email";
        return user;
    }

    public CreateUserInput GetInputWithInvalidCPF()
    {
        var user = GetInput();
        user.CPF = "invalid-cpf";
        return user;
    }

    public CreateUserInput GetInvalidInputShortName()
    {
        var invalidInputShortName = GetInput();
        invalidInputShortName.Name =
            invalidInputShortName.Name[..2];

        return invalidInputShortName;
    }

    public CreateUserInput GetInvalidInputTooLongName()
    {
        var invalidInputTooLongName = GetInput();

        while (invalidInputTooLongName.Name.Length <= 255)
            invalidInputTooLongName.Name = $"{invalidInputTooLongName.Name} {Faker.Commerce.ProductName}";

        return invalidInputTooLongName;
    }

    public CreateUserInput GetInvalidInputWithoutName()
    {
        var invalidInputWithoutName = GetInput();
        invalidInputWithoutName.Name = string.Empty;
        return invalidInputWithoutName;
    }

    public CreateUserInput GetInvalidInputWithoutEmail()
    {
        var invalidInputWithoutEmail = GetInput();
        invalidInputWithoutEmail.Email = string.Empty;
        return invalidInputWithoutEmail;
    }

    public CreateUserInput GetInvalidInputWithoutPhone()
    {
        var invalidInputWithoutPhone = GetInput();
        invalidInputWithoutPhone.Phone = string.Empty;
        return invalidInputWithoutPhone;
    }
}
