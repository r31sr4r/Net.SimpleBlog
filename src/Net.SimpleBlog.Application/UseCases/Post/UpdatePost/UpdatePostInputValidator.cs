using FluentValidation;
using Net.SimpleBlog.Application.UseCases.Post.UpdatePost;

namespace Net.SimpleBlog.Application.UseCases.Post.UpdatePost;
public class UpdatePostInputValidator
    : AbstractValidator<UpdatePostInput>
{
    public UpdatePostInputValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Id must not be empty");
    }
}
