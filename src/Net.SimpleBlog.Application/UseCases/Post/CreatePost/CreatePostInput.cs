using MediatR;
using Net.SimpleBlog.Application.UseCases.Post.Common;

namespace Net.SimpleBlog.Application.UseCases.Post.CreatePost;
public class CreatePostInput : IRequest<PostModelOutput>
{
    public CreatePostInput(string title, string content, Guid userId)
    {
        Title = title;
        Content = content;
        UserId = userId;
    }

    public string Title { get; set; }
    public string Content { get; set; }
    public Guid UserId { get; set; }
}
