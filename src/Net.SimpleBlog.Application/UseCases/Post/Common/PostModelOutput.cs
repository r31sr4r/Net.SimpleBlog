namespace Net.SimpleBlog.Application.UseCases.Post.Common;
public class PostModelOutput
{
    public PostModelOutput(
        Guid id, 
        string title, 
        string content, 
        Guid userId, 
        DateTime createdAt, 
        DateTime? updatedAt = null)
    {
        Id = id;
        Title = title;
        Content = content;
        UserId = userId;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public Guid Id { get; private set; }
    public string Title { get; private set; }
    public string Content { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public static PostModelOutput FromPost(Domain.Entity.Post post)
    {
        return new PostModelOutput(
            post.Id,
            post.Title,
            post.Content,
            post.UserId,
            post.CreatedAt,
            post.UpdatedAt
        );
    }
}
