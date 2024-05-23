namespace Net.SimpleBlog.Api.ApiModels.Post
{
    public class CreatePostApiInput
    {
        public CreatePostApiInput(string title, string content)
        {
            Title = title;
            Content = content;
        }

        public string Title { get; set; }
        public string Content { get; set; }
    }
}
