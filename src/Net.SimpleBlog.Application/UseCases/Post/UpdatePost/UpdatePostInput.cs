using MediatR;
using Net.SimpleBlog.Application.UseCases.Post.Common;

namespace Net.SimpleBlog.Application.UseCases.Post.UpdatePost;
public class UpdatePostInput : IRequest<PostModelOutput>
{
    public UpdatePostInput(Guid id, string title, string content)
    {
        Id = id;
        Title = title;
        Content = content;
    }

    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
}
