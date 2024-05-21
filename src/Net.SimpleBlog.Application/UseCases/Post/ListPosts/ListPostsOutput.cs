using Net.SimpleBlog.Application.Common;
using Net.SimpleBlog.Application.UseCases.Post.Common;

namespace Net.SimpleBlog.Application.UseCases.Post.ListPosts;
public class ListPostsOutput
    : PaginatedListOutput<PostModelOutput>
{
    public ListPostsOutput(
        int page,
        int perPage,
        int total,
        IReadOnlyList<PostModelOutput> items)
        : base(page, perPage, total, items)
    { }
}
