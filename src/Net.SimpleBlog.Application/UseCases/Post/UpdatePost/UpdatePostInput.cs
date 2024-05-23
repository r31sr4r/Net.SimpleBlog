using MediatR;
using Net.SimpleBlog.Application.UseCases.Post.Common;

namespace Net.SimpleBlog.Application.UseCases.Post.UpdatePost;
public class UpdatePostInput : IRequest<PostModelOutput>
{
    public UpdatePostInput(Guid id, string title, string content, Guid userId)
    {
        Id = id;
        Title = title;
        Content = content;
        UserId = userId;
    }

    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public Guid UserId { get; set; }
}
