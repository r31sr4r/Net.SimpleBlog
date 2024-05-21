using FluentAssertions;
using Net.SimpleBlog.Application.UseCases.Post.UpdatePost;
using Xunit;

namespace Net.SimpleBlog.UnitTests.Application.Post.UpdatePost;

[Collection(nameof(UpdatePostTestFixture))]
public class UpdatePostInputValidatorTest
{
    private readonly UpdatePostTestFixture fixture;

    public UpdatePostInputValidatorTest(UpdatePostTestFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact(DisplayName = nameof(DontAcceptWhenGuidIsEmpty))]
    [Trait("Application", "UpdatePostInputValidator - Use Cases")]
    public void DontAcceptWhenGuidIsEmpty()
    {
        var input = fixture.GetValidInput(Guid.Empty);
        var validator = new UpdatePostInputValidator();

        var validateResult = validator.Validate(input);

        validateResult.Should().NotBeNull();
        validateResult.IsValid.Should().BeFalse();
        validateResult.Errors.Should().HaveCount(1);
        validateResult.Errors[0].ErrorMessage
            .Should().Be("Id must not be empty");
    }

    [Fact(DisplayName = nameof(AcceptWhenGuidIsNotEmpty))]
    [Trait("Application", "UpdatePostInputValidator - Use Cases")]
    public void AcceptWhenGuidIsNotEmpty()
    {
        var input = fixture.GetValidInput();
        var validator = new UpdatePostInputValidator();

        var validateResult = validator.Validate(input);

        validateResult.Should().NotBeNull();
        validateResult.IsValid.Should().BeTrue();
        validateResult.Errors.Should().HaveCount(0);
    }
}
