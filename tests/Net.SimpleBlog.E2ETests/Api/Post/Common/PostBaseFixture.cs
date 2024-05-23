using Bogus.Extensions.Brazil;
using Net.SimpleBlog.Application.UseCases.Post.CreatePost;
using Net.SimpleBlog.E2ETests.Base;
using DomainEntity = Net.SimpleBlog.Domain.Entity;

namespace Net.SimpleBlog.E2ETests.Api.Post.Common;

[CollectionDefinition(nameof(PostBaseFixture))]
public class PostBaseFixtureCollection
    : ICollectionFixture<PostBaseFixture>
{ }

public class PostBaseFixture
    : BaseFixture
{
    public PostPersistence Persistence { get; private set; }

    public PostBaseFixture()
        : base()
    {
        Persistence = new PostPersistence(CreateDbContext());
    }

    // Métodos para usuários
    public string GetValidUserName()
    {
        var userName = "";
        while (userName.Length < 3)
            userName = Faker.Internet.UserName();
        if (userName.Length > 255)
            userName = userName[..255];
        return userName;
    }

    public string GetValidEmail()
        => Faker.Internet.Email();

    public string GetValidPhone()
    {
        var phoneNumber = Faker.Random.Bool()
            ? Faker.Phone.PhoneNumber("(##) ####-####")
            : Faker.Phone.PhoneNumber("(##) #####-####");

        return phoneNumber;
    }

    public string GetValidCPF()
        => Faker.Person.Cpf();

    public string GetValidRG()
        => Faker.Person.Random.AlphaNumeric(9);

    public DateTime GetValidDateOfBirth()
        => Faker.Person.DateOfBirth;

    public string GetValidPassword()
        => "ValidPassword123!";

    public bool GetRandomBoolean()
        => new Random().NextDouble() < 0.5;

    public DomainEntity.User GetValidUser()
        => new(
            GetValidUserName(),
            GetValidEmail(),
            GetValidPhone(),
            GetValidCPF(),
            GetValidDateOfBirth(),
            GetValidRG(),
            GetValidPassword()
        );

    public DomainEntity.User GetValidUserWithoutPassword()
        => new(
            GetValidUserName(),
            GetValidEmail(),
            GetValidPhone(),
            GetValidCPF(),
            GetValidDateOfBirth(),
            GetValidRG(),
            string.Empty
        );

    public string GetInvalidEmail() => "invalid-email";

    public string GetInvalidCPF() => "invalid-cpf";

    public string GetInvalidRG() => "invalid-rg";

    public string GetInvalidShortName() => Faker.Internet.UserName()[..2];

    public string GetInvalidTooLongName()
    {
        var invalidTooLongName = Faker.Internet.UserName();
        while (invalidTooLongName.Length <= 255)
            invalidTooLongName = $"{invalidTooLongName} {Faker.Commerce.ProductName()}";
        return invalidTooLongName;
    }

    public List<DomainEntity.User> GeUsersList(int length = 10)
    {
        return Enumerable.Range(1, length)
            .Select(_ => GetValidUser())
            .ToList();
    }

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

    public string GetInvalidShortTitle() => Faker.Lorem.Word()[..2];

    public string GetInvalidTooLongTitle()
    {
        var invalidTooLongTitle = Faker.Lorem.Sentence();
        while (invalidTooLongTitle.Length <= 255)
            invalidTooLongTitle = $"{invalidTooLongTitle} {Faker.Lorem.Word()}";
        return invalidTooLongTitle;
    }

    public string GetInvalidShortContent() => Faker.Lorem.Word()[..2];

    public string GetInvalidTooLongContent()
    {
        var invalidTooLongContent = Faker.Lorem.Paragraph();
        while (invalidTooLongContent.Length <= 10000)
            invalidTooLongContent = $"{invalidTooLongContent} {Faker.Lorem.Paragraph()}";
        return invalidTooLongContent;
    }

    public DomainEntity.Post GetValidPost(Guid userId)
        => new(
            GetValidTitle(),
            GetValidContent(),
            userId
        );

    public List<DomainEntity.Post> GetPostsList(Guid userId, int length = 10)
    {
        return Enumerable.Range(1, length)
            .Select(_ => GetValidPost(userId))
            .ToList();
    }

    public CreatePostInput GetCreatePostInput(Guid userId)
    {
        var post = GetValidPost(userId);
        return new CreatePostInput(
            post.Title,
            post.Content,
            userId
        );
    }
}
