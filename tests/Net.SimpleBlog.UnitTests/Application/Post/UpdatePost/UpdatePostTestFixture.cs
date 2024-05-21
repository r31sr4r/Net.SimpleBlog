using Bogus;
using Net.SimpleBlog.Application.UseCases.Post.UpdatePost;
using Net.SimpleBlog.Application.UseCases.Post.UpdatePost;
using Net.SimpleBlog.UnitTests.Application.Post.Common;
using Xunit;

namespace Net.SimpleBlog.UnitTests.Application.Post.UpdatePost;

[CollectionDefinition(nameof(UpdatePostTestFixture))]
public class UpdatePostTestFixtureCollection
    : ICollectionFixture<UpdatePostTestFixture>
{ }

public class UpdatePostTestFixture
    : PostUseCasesBaseFixture
{
    public UpdatePostInput GetValidInput(Guid? id = null)
        => new UpdatePostInput(
                id ?? Guid.NewGuid(),
                GetValidTitle(),
                GetValidContent()
    );

    public UpdatePostInput GetInvalidInputShortTitle()
    {
        var invalidInputShortTitle = GetValidInput();
        invalidInputShortTitle.Title =
            invalidInputShortTitle.Title[..2];

        return invalidInputShortTitle;
    }

    public UpdatePostInput GetInvalidInputTooLongTitle()
    {
        var invalidInputTooLongTitle = GetValidInput();

        while (invalidInputTooLongTitle.Title.Length <= 255)
            invalidInputTooLongTitle.Title = $"{invalidInputTooLongTitle.Title} {Faker.Lorem.Word()}";

        return invalidInputTooLongTitle;
    }

    public UpdatePostInput GetInvalidInputShortContent()
    {
        var invalidInputShortContent = GetValidInput();
        invalidInputShortContent.Content =
            invalidInputShortContent.Content[..2];

        return invalidInputShortContent;
    }

    public UpdatePostInput GetInvalidInputTooLongContent()
    {
        var invalidInputTooLongContent = GetValidInput();

        while (invalidInputTooLongContent.Content.Length <= 10000)
            invalidInputTooLongContent.Content = $"{invalidInputTooLongContent.Content} {Faker.Lorem.Paragraph()}";

        return invalidInputTooLongContent;
    }
}
