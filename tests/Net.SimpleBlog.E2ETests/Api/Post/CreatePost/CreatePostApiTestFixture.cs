using Net.SimpleBlog.Application.UseCases.Post.CreatePost;
using Net.SimpleBlog.E2ETests.Api.Post.Common;

namespace Net.SimpleBlog.E2ETests.Api.Post.CreatePost;

[CollectionDefinition(nameof(CreatePostApiTestFixture))]
public class CreatePostApiTestFixtureCollection : ICollectionFixture<CreatePostApiTestFixture>
{ }

public class CreatePostApiTestFixture : PostBaseFixture
{
    public CreatePostInput GetInput(Guid userId)
    {
        var post = GetValidPost(userId);
        return new CreatePostInput(
            post.Title,
            post.Content,
            userId
        );
    }

    public CreatePostInput GetInput()
    {
        var user = GetValidUser();
        var post = GetValidPost(user.Id);
        return new CreatePostInput(
            post.Title,
            post.Content,
            user.Id
        );
    }
}
