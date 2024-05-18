using MediatR;
using Net.SimpleBlog.Application.UseCases.User.Common;

namespace Net.SimpleBlog.Application.UseCases.User.Update;
public interface IUpdateUser
    : IRequestHandler<UpdateUserInput, UserModelOutput>
{ }
