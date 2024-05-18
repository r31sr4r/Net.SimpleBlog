using MediatR;
using Net.SimpleBlog.Application.UseCases.User.Common;

namespace Net.SimpleBlog.Application.UseCases.User.GetUser;
public class GetUserInput : IRequest<UserModelOutput>
{
    public GetUserInput(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; set; }
}
