using MediatR;

namespace Net.SimpleBlog.Application.UseCases.Post.ListPosts;
public interface IListPosts : IRequestHandler<ListPostsInput, ListPostsOutput>
{
}
