using MediatR;
using Net.SimpleBlog.Application.Interfaces;
using Net.SimpleBlog.Domain.Repository;

namespace Net.SimpleBlog.Application.UseCases.Post.DeletePost;
public class DeletePost : IDeletePost
{
    private readonly IPostRepository _postRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeletePost(IPostRepository postRepository, IUnitOfWork unitOfWork)
    {
        _postRepository = postRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeletePostInput request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.Get(request.Id, cancellationToken);
        await _postRepository.Delete(post, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);
        return Unit.Value;
    }
}
