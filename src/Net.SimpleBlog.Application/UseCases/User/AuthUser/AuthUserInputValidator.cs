using FluentValidation;

namespace Net.SimpleBlog.Application.UseCases.User.AuthUser;
public class AuthUserInputValidator
    : AbstractValidator<AuthUserInput>
{
    public AuthUserInputValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Email must be valid.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required."); ;
    }
}