using MediatR;
using Net.SimpleBlog.Application.UseCases.User.Common;

namespace Net.SimpleBlog.Application.UseCases.User.GetUser;
public interface IGetUser :
    IRequestHandler<GetUserInput, UserModelOutput>
{
}

