using Net.SimpleBlog.Application.UseCases.Post.CreatePost;
using Net.SimpleBlog.IntegrationTests.Application.UseCases.Post.Common;

namespace Net.SimpleBlog.IntegrationTests.Application.UseCases.Post.CreatePost;

[CollectionDefinition(nameof(CreatePostTestFixture))]
public class CreatePostTestFixtureCollection : ICollectionFixture<CreatePostTestFixture>
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

    public CreatePostInput GetInvalidInputShortTitle()
    {
        var invalidInputShortTitle = GetInput();
        invalidInputShortTitle.Title = invalidInputShortTitle.Title[..2];
        return invalidInputShortTitle;
    }

    public CreatePostInput GetInvalidInputTooLongTitle()
    {
        var invalidInputTooLongTitle = GetInput();
        while (invalidInputTooLongTitle.Title.Length <= 255)
            invalidInputTooLongTitle.Title = $"{invalidInputTooLongTitle.Title} {Faker.Lorem.Word()}";
        return invalidInputTooLongTitle;
    }

    public CreatePostInput GetInvalidInputShortContent()
    {
        var invalidInputShortContent = GetInput();
        invalidInputShortContent.Content = invalidInputShortContent.Content[..2];
        return invalidInputShortContent;
    }

    public CreatePostInput GetInvalidInputTooLongContent()
    {
        var invalidInputTooLongContent = GetInput();
        while (invalidInputTooLongContent.Content.Length <= 10000)
            invalidInputTooLongContent.Content = $"{invalidInputTooLongContent.Content} {Faker.Lorem.Paragraph()}";
        return invalidInputTooLongContent;
    }
}
