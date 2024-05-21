using Net.SimpleBlog.UnitTests.Application.Post.Common;
using Xunit;

namespace Net.SimpleBlog.UnitTests.Application.Post.GetPost;

[CollectionDefinition(nameof(GetPostTestFixture))]
public class GetPostTestFixtureCollection :
    ICollectionFixture<GetPostTestFixture>
{ }

public class GetPostTestFixture : PostUseCasesBaseFixture
{ }
