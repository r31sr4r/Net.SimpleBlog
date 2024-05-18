using Net.SimpleBlog.Application.Interfaces;
using Net.SimpleBlog.Application.UseCases.User.Common;
using Net.SimpleBlog.Domain.Repository;

namespace Net.SimpleBlog.Application.UseCases.User.Update;
public class UpdateUser
    : IUpdateUser
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUser(
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
        => (_userRepository, _unitOfWork)
            = (userRepository, unitOfWork);


    public async Task<UserModelOutput> Handle(UpdateUserInput request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.Get(request.Id, cancellationToken);
        user.Update(
            request.Name,
            request.Email,
            request.Phone,
            request.CPF,
            request.DateOfBirth,
            request.RG
            );

        if (request.IsActive != user.IsActive)
            if ((bool)request.IsActive!) user.Activate();
            else user.Deactivate();

        await _userRepository.Update(user, cancellationToken);
        await _unitOfWork.Commit(cancellationToken);

        return UserModelOutput.FromUser(user);

    }
}

