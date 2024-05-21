using Net.SimpleBlog.IntegrationTests.Application.UseCases.Post.Common;

namespace Net.SimpleBlog.IntegrationTests.Application.UseCases.Post.DeletePost;
[CollectionDefinition(nameof(DeletePostTestFixture))]
public class DeletePostTestFixtureCollection
    : ICollectionFixture<DeletePostTestFixture>
{ }

public class DeletePostTestFixture
    : PostUseCasesBaseFixture
{ }
