using MediatR;
using Net.SimpleBlog.Application.UseCases.User.Common;

namespace Net.SimpleBlog.Application.UseCases.User.AuthUser;
public interface IAuthUser :
    IRequestHandler<AuthUserInput, UserModelOutput>
{
}
