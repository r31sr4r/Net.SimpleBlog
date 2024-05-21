using MediatR;
using Net.SimpleBlog.Application.Common;
using Net.SimpleBlog.Domain.SeedWork.SearchableRepository;

namespace Net.SimpleBlog.Application.UseCases.Post.ListPosts;
public class ListPostsInput
    : PaginatedListInput,
    IRequest<ListPostsOutput>
{
    public ListPostsInput(
        int page = 1,
        int perPage = 15,
        string search = "",
        string sort = "",
        SearchOrder dir = SearchOrder.Asc)
        : base(page, perPage, search, sort, dir)
    { }

    public ListPostsInput()
        : base(1, 15, "", "", SearchOrder.Asc)
    { }
}
