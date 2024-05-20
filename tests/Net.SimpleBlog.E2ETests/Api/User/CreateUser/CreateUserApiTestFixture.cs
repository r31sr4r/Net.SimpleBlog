using Net.SimpleBlog.Application.UseCases.User.CreateUser;
using Net.SimpleBlog.E2ETests.Api.User.Common;

namespace Net.SimpleBlog.E2ETests.Api.User.CreateUser;

[CollectionDefinition(nameof(CreateUserApiTestFixture))]
public class CreateUserApiTestFixtureCollection
: ICollectionFixture<CreateUserApiTestFixture>
{}

public class CreateUserApiTestFixture
    : UserBaseFixture
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
}
