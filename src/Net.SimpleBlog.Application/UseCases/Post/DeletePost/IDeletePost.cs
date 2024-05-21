using MediatR;

namespace Net.SimpleBlog.Application.UseCases.Post.DeletePost;
public interface IDeletePost : IRequestHandler<DeletePostInput>
{
}
