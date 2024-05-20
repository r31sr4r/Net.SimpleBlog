using Net.SimpleBlog.E2ETests.Api.User.Common;

namespace Net.SimpleBlog.E2ETests.Api.User.GetUser;

[CollectionDefinition(nameof(GetUserApiTestFixture))]
public class GetUserApiTestFixtureCollection
: ICollectionFixture<GetUserApiTestFixture>
{ }

public class GetUserApiTestFixture
    : UserBaseFixture
{

}
