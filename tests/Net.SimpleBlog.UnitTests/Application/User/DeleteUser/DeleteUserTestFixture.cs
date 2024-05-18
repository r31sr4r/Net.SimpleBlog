using Net.SimpleBlog.UnitTests.Application.User.Common;
using Xunit;

namespace Net.SimpleBlog.UnitTests.Application.User.DeleteUser;
[CollectionDefinition(nameof(DeleteUserTestFixture))]
public class DeleteUserTestFixtureCollection
    : ICollectionFixture<DeleteUserTestFixture>
{ }
public class DeleteUserTestFixture
    : UserUseCasesBaseFixture
{ }