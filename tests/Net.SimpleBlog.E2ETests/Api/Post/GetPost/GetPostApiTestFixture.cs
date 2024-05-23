using Net.SimpleBlog.E2ETests.Api.Post.Common;

namespace Net.SimpleBlog.E2ETests.Api.Post.GetPost;

[CollectionDefinition(nameof(GetPostApiTestFixture))]
public class GetPostApiTestFixtureCollection : ICollectionFixture<GetPostApiTestFixture>
{ }

public class GetPostApiTestFixture : PostBaseFixture
{
}
