using Net.SimpleBlog.Application.Interfaces;
using Net.SimpleBlog.Application.UseCases.Post.Common;
using Net.SimpleBlog.Domain.Repository;
using DomainEntity = Net.SimpleBlog.Domain.Entity;

namespace Net.SimpleBlog.Application.UseCases.Post.CreatePost;
public class CreatePost : ICreatePost
{
    private readonly IPostRepository _postRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreatePost(IPostRepository postRepository, IUnitOfWork unitOfWork)
    {
        _postRepository = postRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<PostModelOutput> Handle(CreatePostInput request, CancellationToken cancellationToken)
    {
        var post = new DomainEntity.Post(
            request.Title,
            request.Content,
            request.UserId
        );

        await _postRepository.Insert(post, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);

        return PostModelOutput.FromPost(post);
    }
}
