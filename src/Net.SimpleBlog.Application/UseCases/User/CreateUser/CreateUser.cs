using Net.SimpleBlog.Application.Interfaces;
using Net.SimpleBlog.Application.UseCases.User.Common;
using Net.SimpleBlog.Domain.Repository;
using DomainEntity = Net.SimpleBlog.Domain.Entity;


namespace Net.SimpleBlog.Application.UseCases.User.CreateUser;
public class CreateUser : ICreateUser
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateUser(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork
    )
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<UserModelOutput> Handle(CreateUserInput request, CancellationToken cancellationToken)
    {
        var user = new DomainEntity.User(
            request.Name,
            request.Email,
            request.Phone,
            request.CPF,
            request.DateOfBirth,
            request.RG,
            request.Password
        );

        await _userRepository.Insert(user, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);

        return UserModelOutput.FromUser(user);
    }
}

