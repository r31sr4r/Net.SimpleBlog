using MediatR;

namespace Net.SimpleBlog.Application.UseCases.User.ListUsers;
public interface IListUsers
    : IRequestHandler<ListUsersInput, ListUsersOutput>
{
}
