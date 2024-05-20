using Net.SimpleBlog.Domain.Exceptions;
using Net.SimpleBlog.Domain.SeedWork;
using Net.SimpleBlog.Domain.Validation;

namespace Net.SimpleBlog.Domain.Entity;

public class Post : AggregateRoot
{
    public Post() { }

    public Post(string title, string content, Guid userId) : base()
    {
        Title = title;
        Content = content;
        UserId = userId;
        CreatedAt = DateTime.Now;
        Validate();
    }

    public string Title { get; private set; }
    public string Content { get; private set; }
    public Guid UserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    public void Update(string title, string content)
    {
        Title = title;
        Content = content;
        UpdatedAt = DateTime.Now;
        Validate();
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Title))
            throw new EntityValidationException($"{nameof(Title)} should not be empty or null");
        if (Title.Length <= 3)
            throw new EntityValidationException($"{nameof(Title)} should be greater than 3 characters");
        if (Title.Length >= 255)
            throw new EntityValidationException($"{nameof(Title)} should be less than 255 characters");
        if (string.IsNullOrWhiteSpace(Content))
            throw new EntityValidationException($"{nameof(Content)} should not be empty or null");
    }
}
