using FluentValidation;

namespace Net.SimpleBlog.Application.UseCases.Post.GetPost;

public class GetPostInputValidator : AbstractValidator<GetPostInput>
{
    public GetPostInputValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();
    }
}
