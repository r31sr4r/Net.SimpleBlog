using MediatR;
using Net.SimpleBlog.Application.UseCases.Post.Common;

namespace Net.SimpleBlog.Application.UseCases.Post.CreatePost;
public interface ICreatePost : IRequestHandler<CreatePostInput, PostModelOutput>
{
}
