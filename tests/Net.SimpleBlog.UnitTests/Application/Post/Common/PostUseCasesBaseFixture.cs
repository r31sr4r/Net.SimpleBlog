using Bogus;
using Moq;
using Net.SimpleBlog.Application.Interfaces;
using Net.SimpleBlog.Domain.Repository;
using Net.SimpleBlog.UnitTests.Common;
using DomainEntity = Net.SimpleBlog.Domain.Entity;

namespace Net.SimpleBlog.UnitTests.Application.Post.Common;

public class PostUseCasesBaseFixture : BaseFixture
{
    public Mock<IPostRepository> GetRepositoryMock() => new();

    public Mock<IUnitOfWork> GetUnitOfWorkMock() => new();

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
        => Faker.Lorem.Paragraphs();

    public Guid GetValidUserId()
        => Guid.NewGuid();

    public DomainEntity.Post GetValidPost()
        => new(
            GetValidTitle(),
            GetValidContent(),
            GetValidUserId()
        );

    public List<DomainEntity.Post> GetExamplePostList(int length = 10)
    {
        return Enumerable.Range(1, length)
            .Select(_ => GetValidPost())
            .ToList();
    }
}
