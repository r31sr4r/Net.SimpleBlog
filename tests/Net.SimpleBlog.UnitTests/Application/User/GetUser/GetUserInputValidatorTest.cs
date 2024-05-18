using FluentAssertions;
using Net.SimpleBlog.Application.UseCases.User.GetUser;
using Xunit;

namespace Net.SimpleBlog.UnitTests.Application.User.GetUser;
[Collection(nameof(GetUserTestFixture))]
public class GetUserInputValidatorTest
{
    private readonly GetUserTestFixture _fixture;

    public GetUserInputValidatorTest(GetUserTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(ValidationOk))]
    [Trait("Application", "GetUser - Use Cases")]
    public void ValidationOk()
    {
        var validInput = new GetUserInput(Guid.NewGuid());
        var validator = new GetUserInputValidator();

        var result = validator.Validate(validInput);

        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.Errors.Should().HaveCount(0);
    }


    [Fact(DisplayName = nameof(ValidationExceptionWhenIdIsEmpty))]
    [Trait("Application", "GetUser - Use Cases")]
    public void ValidationExceptionWhenIdIsEmpty()
    {
        var invalidInput = new GetUserInput(Guid.Empty);
        var validator = new GetUserInputValidator();

        var result = validator.Validate(invalidInput);

        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.Errors.Should().HaveCount(1);
        result.Errors[0].ErrorMessage.Should().Be("'Id' must not be empty.");
    }

}
