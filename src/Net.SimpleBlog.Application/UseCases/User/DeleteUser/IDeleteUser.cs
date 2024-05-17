using MediatR;

namespace Net.SimpleBlog.Application.UseCases.User.DeleteUser;
public interface IDeleteUser
    : IRequestHandler<DeleteUserInput>
{ }
