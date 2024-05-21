using Net.SimpleBlog.Application.UseCases.Post.Common;
using Net.SimpleBlog.Domain.Repository;

namespace Net.SimpleBlog.Application.UseCases.Post.GetPost;
public class GetPost : IGetPost
{
    private readonly IPostRepository _postRepository;

    public GetPost(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<PostModelOutput> Handle(GetPostInput request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.Get(request.Id, cancellationToken);
        return PostModelOutput.FromPost(post);
    }
}
