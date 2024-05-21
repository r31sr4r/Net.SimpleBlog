using MediatR;
using Net.SimpleBlog.Application.UseCases.Post.Common;

namespace Net.SimpleBlog.Application.UseCases.Post.GetPost;
public class GetPostInput : IRequest<PostModelOutput>
{
    public GetPostInput(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; set; }
}
