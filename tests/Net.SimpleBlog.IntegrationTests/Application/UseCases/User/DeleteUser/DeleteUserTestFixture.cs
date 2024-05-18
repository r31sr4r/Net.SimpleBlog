using Net.SimpleBlog.IntegrationTests.Application.UseCases.User.Common;

namespace Net.SimpleBlog.IntegrationTests.Application.UseCases.User.DeleteUser;
[CollectionDefinition(nameof(DeleteUserTestFixture))]
public class DeleteUserTestFixtureCollection
    : ICollectionFixture<DeleteUserTestFixture>
{ }

public class DeleteUserTestFixture
    : UserUseCasesBaseFixture
{ }
