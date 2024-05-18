using FluentAssertions;
using Net.SimpleBlog.Application.UseCases.User.AuthUser;
using Xunit;

namespace Net.SimpleBlog.UnitTests.Application.User.AuthUser;
public class AuthUserInputValidatorTest
{
    [Fact(DisplayName = nameof(RejectWhenEmailIsEmpty))]
    [Trait("Application", "AuthUserInputValidator - Use Cases")]
    public void RejectWhenEmailIsEmpty()
    {
        var validator = new AuthUserInputValidator();
        var input = new AuthUserInput("", "ValidPassword123!");

        var validationResult = validator.Validate(input);

        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().Contain(e => e.ErrorMessage == "Email is required.");
    }

    [Fact(DisplayName = nameof(RejectWhenEmailIsInvalid))]
    [Trait("Application", "AuthUserInputValidator - Use Cases")]
    public void RejectWhenEmailIsInvalid()
    {
        var validator = new AuthUserInputValidator();
        var input = new AuthUserInput("invalidemail", "ValidPassword123!");

        var validationResult = validator.Validate(input);

        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().Contain(e => e.ErrorMessage == "Email must be valid.");
    }

    [Fact(DisplayName = nameof(RejectWhenPasswordIsEmpty))]
    [Trait("Application", "AuthUserInputValidator - Use Cases")]
    public void RejectWhenPasswordIsEmpty()
    {
        var validator = new AuthUserInputValidator();
        var input = new AuthUserInput("user@example.com", "");

        var validationResult = validator.Validate(input);

        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeFalse();
        validationResult.Errors.Should().Contain(e => e.ErrorMessage == "Password is required.");
    }

    [Fact(DisplayName = nameof(AcceptWhenInputIsValid))]
    [Trait("Application", "AuthUserInputValidator - Use Cases")]
    public void AcceptWhenInputIsValid()
    {
        var validator = new AuthUserInputValidator();
        var input = new AuthUserInput("user@example.com", "ValidPassword123!");

        var validationResult = validator.Validate(input);

        validationResult.Should().NotBeNull();
        validationResult.IsValid.Should().BeTrue();
        validationResult.Errors.Should().HaveCount(0);
    }
}

