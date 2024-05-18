using Net.SimpleBlog.UnitTests.Application.User.Common;
using Xunit;

namespace Net.SimpleBlog.UnitTests.Application.User.AuthUser;
[CollectionDefinition(nameof(AuthUserTestFixture))]
public class AuthUserTestFixtureCollection :
    ICollectionFixture<AuthUserTestFixture>
{ }

public class AuthUserTestFixture
    : UserUseCasesBaseFixture
{ }

