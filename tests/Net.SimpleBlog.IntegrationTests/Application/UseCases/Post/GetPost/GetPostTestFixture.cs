using Net.SimpleBlog.IntegrationTests.Application.UseCases.Post.Common;

namespace Net.SimpleBlog.IntegrationTests.Application.UseCases.Post.GetPost;

[CollectionDefinition(nameof(GetPostTestFixture))]
public class GetPostTestFixtureCollection
    : ICollectionFixture<GetPostTestFixture>
{ }

public class GetPostTestFixture
    : PostUseCasesBaseFixture
{ }
