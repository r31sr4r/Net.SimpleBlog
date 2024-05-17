using MediatR;
using Net.SimpleBlog.Application.UseCases.User.Common;

namespace Net.SimpleBlog.Application.UseCases.User.CreateUser;
public interface ICreateUser :
    IRequestHandler<CreateUserInput, UserModelOutput>
{
}
