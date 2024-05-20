using Net.SimpleBlog.Application.UseCases.User.Update;
using Net.SimpleBlog.E2ETests.Api.User.Common;

namespace Net.SimpleBlog.E2ETests.Api.User.UpdateUser;

[CollectionDefinition(nameof(UpdateUserApiTestFixture))]
public class UpdateUserApiTestFixtureCollection
: ICollectionFixture<UpdateUserApiTestFixture>
{ }

public class UpdateUserApiTestFixture
    : UserBaseFixture
{
    public UpdateUserInput GetInput(Guid? id = null)
    {
        var user = GetValidUserWithoutPassword();
        return new UpdateUserInput(
            id ?? Guid.NewGuid(),
            user.Name,
            user.Email,
            user.Phone,
            user.CPF,
            user.DateOfBirth,
            user.RG,
            GetRandomBoolean()
        );
    }
}
