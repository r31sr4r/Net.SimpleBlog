using MediatR;

namespace Net.SimpleBlog.Application.UseCases.Post.DeletePost;
public class DeletePostInput : IRequest
{
    public DeletePostInput(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; set; }
}
