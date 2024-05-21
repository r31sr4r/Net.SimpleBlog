using FluentAssertions;
using Net.SimpleBlog.Application.UseCases.Post.GetPost;
using Xunit;

namespace Net.SimpleBlog.UnitTests.Application.Post.GetPost;

[Collection(nameof(GetPostTestFixture))]
public class GetPostInputValidatorTest
{
    private readonly GetPostTestFixture _fixture;

    public GetPostInputValidatorTest(GetPostTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(ValidationOk))]
    [Trait("Application", "GetPost - Use Cases")]
    public void ValidationOk()
    {
        var validInput = new GetPostInput(Guid.NewGuid());
        var validator = new GetPostInputValidator();

        var result = validator.Validate(validInput);

        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Errors.Should().HaveCount(0);
    }

    [Fact(DisplayName = nameof(ValidationExceptionWhenIdIsEmpty))]
    [Trait("Application", "GetPost - Use Cases")]
    public void ValidationExceptionWhenIdIsEmpty()
    {
        var invalidInput = new GetPostInput(Guid.Empty);
        var validator = new GetPostInputValidator();

        var result = validator.Validate(invalidInput);

        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].ErrorMessage.Should().Be("'Id' must not be empty.");
    }
}
