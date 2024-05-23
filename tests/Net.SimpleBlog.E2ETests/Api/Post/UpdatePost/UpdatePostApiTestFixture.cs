using Net.SimpleBlog.Application.UseCases.Post.CreatePost;
using Net.SimpleBlog.Application.UseCases.Post.UpdatePost;
using Net.SimpleBlog.E2ETests.Api.Post.Common;

namespace Net.SimpleBlog.E2ETests.Api.Post.UpdatePost;

[CollectionDefinition(nameof(UpdatePostApiTestFixture))]
public class UpdatePostApiTestFixtureCollection : ICollectionFixture<UpdatePostApiTestFixture>
{ }

public class UpdatePostApiTestFixture : PostBaseFixture
{
    public UpdatePostInput GetInput(Guid userId)
    {
        var post = GetValidPost(userId);
        return new UpdatePostInput(
            post.Id,
            post.Title,
            post.Content,
            userId
        );
    }
}
