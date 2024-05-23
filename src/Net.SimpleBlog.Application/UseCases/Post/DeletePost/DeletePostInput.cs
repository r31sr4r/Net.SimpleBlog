using MediatR;

namespace Net.SimpleBlog.Application.UseCases.Post.DeletePost;
public class DeletePostInput : IRequest
{
    public DeletePostInput(Guid id, Guid userId)
    {
        Id = id;
        UserId = userId;
    }

    public Guid Id { get; set; }
    public Guid UserId { get; set; }
}
