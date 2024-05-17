using Net.SimpleBlog.Application.Exceptions;
using Net.SimpleBlog.Application.UseCases.User.Common;
using Net.SimpleBlog.Domain.Common.Security;
using Net.SimpleBlog.Domain.Repository;

namespace Net.SimpleBlog.Application.UseCases.User.AuthUser;
public class AuthUser : IAuthUser
{
    private readonly IUserRepository _userRepository;

    public AuthUser(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserModelOutput> Handle(AuthUserInput request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmail(request.Email, cancellationToken);
        if (user == null)
            throw new CustomAuthenticationException("Invalid email or password.");

        if (!PasswordHasher.VerifyPasswordHash(request.Password, user.Password!))
            throw new CustomAuthenticationException("Invalid email or password.");

        return UserModelOutput.FromUser(user);
    }
}