using Net.SimpleBlog.Application.UseCases.User.Common;
using Net.SimpleBlog.Domain.Repository;

namespace Net.SimpleBlog.Application.UseCases.User.GetUser;
public class GetUser : IGetUser
{
    private readonly IUserRepository _repository;

    public GetUser(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<UserModelOutput> Handle(
        GetUserInput request,
        CancellationToken cancellationToken
        )
    {
        var user = await _repository.Get(request.Id, cancellationToken);
        return UserModelOutput.FromUser(user);

    }
}

