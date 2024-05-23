using Net.SimpleBlog.E2ETests.Api.Post.Common;

namespace Net.SimpleBlog.E2ETests.Api.Post.DeletePost;

[CollectionDefinition(nameof(DeletePostApiTestFixture))]
public class DeletePostApiTestFixtureCollection
    : ICollectionFixture<DeletePostApiTestFixture>
{ }

public class DeletePostApiTestFixture
    : PostBaseFixture
{
}
