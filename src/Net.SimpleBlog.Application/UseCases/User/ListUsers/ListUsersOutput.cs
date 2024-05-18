using Net.SimpleBlog.Application.Common;
using Net.SimpleBlog.Application.UseCases.User.Common;

namespace Net.SimpleBlog.Application.UseCases.User.ListUsers;
public class ListUsersOutput
    : PaginatedListOutput<UserModelOutput>
{
    public ListUsersOutput(
        int page,
        int perPage,
        int total,
        IReadOnlyList<UserModelOutput> items)
        : base(page, perPage, total, items)
    { }
}
