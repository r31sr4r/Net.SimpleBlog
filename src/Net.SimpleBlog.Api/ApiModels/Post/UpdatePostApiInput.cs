namespace Net.SimpleBlog.Api.ApiModels.Post;

public class UpdatePostApiInput
{
    public UpdatePostApiInput(
        string title,
        string content
    )
    {
        Title = title;
        Content = content;
    }

    public string Title { get; private set; }
    public string Content { get; private set; }
}
