using Bogus;
using Net.SimpleBlog.IntegrationTests.Base;
using DomainEntity = Net.SimpleBlog.Domain.Entity;

namespace Net.SimpleBlog.IntegrationTests.Application.UseCases.Post.Common;
public class PostUseCasesBaseFixture : BaseFixture
{
    public string GetValidTitle()
    {
        var title = "";
        while (title.Length < 3)
            title = Faker.Lorem.Sentence();
        if (title.Length > 255)
            title = title[..255];
        return title;
    }

    public string GetValidContent()
    {
        var content = "";
        while (content.Length < 3)
            content = Faker.Lorem.Paragraph();
        if (content.Length > 10000)
            content = content[..10000];
        return content;
    }

    public Guid GetValidUserId() => Guid.NewGuid();

    public DomainEntity.Post GetValidPost()
        => new(
            GetValidTitle(),
            GetValidContent(),
            GetValidUserId()
        );

    public List<DomainEntity.Post> GetPostsList(int length = 10)
    {
        return Enumerable.Range(1, length)
            .Select(_ => GetValidPost())
            .ToList();
    }
}
