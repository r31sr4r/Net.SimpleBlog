using Net.SimpleBlog.Application.UseCases.User.Common;
using Net.SimpleBlog.Domain.Repository;

namespace Net.SimpleBlog.Application.UseCases.User.ListUsers;
public class ListUsers
    : IListUsers
{
    private readonly IUserRepository _categoryRepository;

    public ListUsers(IUserRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<ListUsersOutput> Handle(
        ListUsersInput request,
        CancellationToken cancellationToken)
    {
        var searchOutput = await _categoryRepository.Search(
            new(
                request.Page,
                request.PerPage,
                request.Search,
                request.Sort,
                request.Dir
                ),
            cancellationToken
        );
        return new ListUsersOutput(
            searchOutput.CurrentPage,
            searchOutput.PerPage,
            searchOutput.Total,
            searchOutput.Items
                .Select(UserModelOutput.FromUser)
                .ToList()
        );
    }
}

