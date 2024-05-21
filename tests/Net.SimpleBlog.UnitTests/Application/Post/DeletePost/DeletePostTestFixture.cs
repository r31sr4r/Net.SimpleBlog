using Net.SimpleBlog.UnitTests.Application.Post.Common;
using Xunit;

namespace Net.SimpleBlog.UnitTests.Application.Post.DeletePost;

[CollectionDefinition(nameof(DeletePostTestFixture))]
public class DeletePostTestFixtureCollection
    : ICollectionFixture<DeletePostTestFixture>
{ }

public class DeletePostTestFixture
    : PostUseCasesBaseFixture
{ }
