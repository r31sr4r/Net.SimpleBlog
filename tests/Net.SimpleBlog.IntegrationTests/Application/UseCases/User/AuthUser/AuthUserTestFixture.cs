using Net.SimpleBlog.IntegrationTests.Application.UseCases.User.Common;

namespace Net.SimpleBlog.IntegrationTests.Application.UseCases.User.AuthUser;

[CollectionDefinition(nameof(AuthUserTestFixture))]
public class AuthUserTestFixtureCollection
    : ICollectionFixture<AuthUserTestFixture>
{ }

public class AuthUserTestFixture
    : UserUseCasesBaseFixture
{ }
