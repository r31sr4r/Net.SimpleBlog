using FluentAssertions;
using Net.SimpleBlog.Application.UseCases.User.Update;
using Xunit;

namespace Net.SimpleBlog.UnitTests.Application.User.UpdateUser;
[Collection(nameof(UpdateUserTestFixture))]
public class UpdateUserInputValidatorTest
{
    private readonly UpdateUserTestFixture fixture;

    public UpdateUserInputValidatorTest(UpdateUserTestFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact(DisplayName = nameof(DontAcceptWhenGuidIsEmpty))]
    [Trait("Application", "UpdateUserInputValidator - Use Cases")]
    public void DontAcceptWhenGuidIsEmpty()
    {
        var input = fixture.GetValidInput(Guid.Empty);
        var validator = new UpdateUserInputValidator();

        var validateResult = validator.Validate(input);

        validateResult.Should().NotBeNull();
        validateResult.IsValid.Should().BeFalse();
        validateResult.Errors.Should().HaveCount(1);
        validateResult.Errors[0].ErrorMessage
            .Should().Be("Id must not be empty");
    }

    [Fact(DisplayName = nameof(AcceptWhenGuidIsNotEmpty))]
    [Trait("Application", "UpdateUserInputValidator - Use Cases")]
    public void AcceptWhenGuidIsNotEmpty()
    {
        var input = fixture.GetValidInput();
        var validator = new UpdateUserInputValidator();

        var validateResult = validator.Validate(input);

        validateResult.Should().NotBeNull();
        validateResult.IsValid.Should().BeTrue();
        validateResult.Errors.Should().HaveCount(0);
    }
}
