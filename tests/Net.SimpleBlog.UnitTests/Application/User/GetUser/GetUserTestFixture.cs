using Net.SimpleBlog.UnitTests.Application.User.Common;
using Xunit;

namespace Net.SimpleBlog.UnitTests.Application.User.GetUser;
[CollectionDefinition(nameof(GetUserTestFixture))]
public class GetUserTestFixtureCollection :
    ICollectionFixture<GetUserTestFixture>
{ }

public class GetUserTestFixture
    : UserUseCasesBaseFixture
{ }
