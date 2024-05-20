using Net.SimpleBlog.E2ETests.Api.User.Common;

namespace Net.SimpleBlog.E2ETests.Api.User.AuthUser;
[CollectionDefinition(nameof(AuthUserApiTestFixture))]
public class AuthUserApiTestFixtureCollection
: ICollectionFixture<AuthUserApiTestFixture>
{ }

public class AuthUserApiTestFixture
    : UserBaseFixture
{
}
