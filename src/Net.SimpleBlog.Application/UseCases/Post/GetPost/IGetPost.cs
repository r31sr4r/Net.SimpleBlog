using MediatR;
using Net.SimpleBlog.Application.UseCases.Post.Common;

namespace Net.SimpleBlog.Application.UseCases.Post.GetPost;
public interface IGetPost : IRequestHandler<GetPostInput, PostModelOutput>
{
}
