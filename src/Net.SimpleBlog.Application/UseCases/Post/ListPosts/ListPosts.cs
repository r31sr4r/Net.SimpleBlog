using Net.SimpleBlog.Application.UseCases.Post.Common;
using Net.SimpleBlog.Domain.Repository;

namespace Net.SimpleBlog.Application.UseCases.Post.ListPosts;
public class ListPosts : IListPosts
{
    private readonly IPostRepository _postRepository;

    public ListPosts(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<ListPostsOutput> Handle(
        ListPostsInput request,
        CancellationToken cancellationToken)
    {
        var searchOutput = await _postRepository.Search(
            new(
                request.Page,
                request.PerPage,
                request.Search,
                request.Sort,
                request.Dir
                ),
            cancellationToken
        );
        return new ListPostsOutput(
            searchOutput.CurrentPage,
            searchOutput.PerPage,
            searchOutput.Total,
            searchOutput.Items
                .Select(PostModelOutput.FromPost)
                .ToList()
        );
    }
}
