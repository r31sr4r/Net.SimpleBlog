using Net.SimpleBlog.Application.Interfaces;
using Net.SimpleBlog.Application.UseCases.Post.Common;
using Net.SimpleBlog.Domain.Repository;
using DomainEntity = Net.SimpleBlog.Domain.Entity;

namespace Net.SimpleBlog.Application.UseCases.Post.UpdatePost;
public class UpdatePost : IUpdatePost
{
    private readonly IPostRepository _postRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePost(IPostRepository postRepository, IUnitOfWork unitOfWork)
    {
        _postRepository = postRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PostModelOutput> Handle(UpdatePostInput request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.Get(request.Id, cancellationToken);

        if (post.UserId != request.UserId)
        {
            throw new UnauthorizedAccessException("You are not the owner of this post.");
        }

        post.Update(request.Title, request.Content);

        await _postRepository.Update(post, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);

        return PostModelOutput.FromPost(post);
    }
}
