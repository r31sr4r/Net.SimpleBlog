using Net.SimpleBlog.Application.UseCases.Post.CreatePost;
using Net.SimpleBlog.UnitTests.Application.Post.Common;
using Xunit;

namespace Net.SimpleBlog.UnitTests.Application.Post.CreatePost;

[CollectionDefinition(nameof(CreatePostTestFixture))]
public class CreatePostTestFixtureCollection
    : ICollectionFixture<CreatePostTestFixture>
{ }

public class CreatePostTestFixture : PostUseCasesBaseFixture
{
    public CreatePostInput GetInput()
    {
        var post = GetValidPost();
        return new CreatePostInput(
            post.Title,
            post.Content,
            post.UserId
        );
    }

    public CreatePostInput GetInputWithInvalidTitle()
    {
        var input = GetInput();
        input.Title = "ab"; // Title too short
        return input;
    }

    public CreatePostInput GetInputWithInvalidContent()
    {
        var input = GetInput();
        input.Content = ""; // Content is empty
        return input;
    }
}
