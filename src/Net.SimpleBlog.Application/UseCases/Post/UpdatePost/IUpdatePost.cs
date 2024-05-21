using MediatR;
using Net.SimpleBlog.Application.UseCases.Post.Common;

namespace Net.SimpleBlog.Application.UseCases.Post.UpdatePost;
public interface IUpdatePost : IRequestHandler<UpdatePostInput, PostModelOutput>
{
}
