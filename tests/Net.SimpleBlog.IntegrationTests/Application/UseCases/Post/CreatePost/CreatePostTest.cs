using Net.SimpleBlog.Application.UseCases.Post.CreatePost;
using Net.SimpleBlog.Domain.Exceptions;
using Net.SimpleBlog.Infra.Data.EF;
using Net.SimpleBlog.Infra.Data.EF.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using UseCase = Net.SimpleBlog.Application.UseCases.Post.CreatePost;

namespace Net.SimpleBlog.IntegrationTests.Application.UseCases.Post.CreatePost;

[Collection(nameof(CreatePostTestFixture))]
public class CreatePostTest
{
    private readonly CreatePostTestFixture _fixture;

    public CreatePostTest(CreatePostTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(CreatePost))]
    [Trait("Integration/Application", "Create Post - Use Cases")]
    public async void CreatePost()
    {
        var dbContext = _fixture.CreateDbContext();
        var repository = new PostRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);

        var useCase = new UseCase.CreatePost(repository, unitOfWork);

        var input = _fixture.GetInput();

        var output = await useCase.Handle(input, CancellationToken.None);

        var dbPost = await (_fixture.CreateDbContext(true))
            .Posts.FindAsync(output.Id);

        dbPost.Should().NotBeNull();
        dbPost!.Title.Should().Be(input.Title);
        dbPost.Content.Should().Be(input.Content);
        dbPost.UserId.Should().Be(input.UserId);

        output.Should().NotBeNull();
        output.Title.Should().Be(input.Title);
        output.Content.Should().Be(input.Content);
        output.UserId.Should().Be(input.UserId);
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBeSameDateAs(default);
    }

    [Theory(DisplayName = nameof(ThrowWhenCantInstantiatePost))]
    [Trait("Integration/Application", "Create Post - Use Cases")]
    [MemberData(
        nameof(CreatePostTestDataGenerator.GetInvalidInputs),
        parameters: 6,
        MemberType = typeof(CreatePostTestDataGenerator)
    )]
    public async void ThrowWhenCantInstantiatePost(
        CreatePostInput input,
        string expectedMessage
    )
    {
        var dbContext = _fixture.CreateDbContext();
        var repository = new PostRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);

        var useCase = new UseCase.CreatePost(repository, unitOfWork);

        Func<Task> task = async () => await useCase.Handle(input, CancellationToken.None);

        await task.Should()
            .ThrowAsync<EntityValidationException>()
            .WithMessage(expectedMessage);

        var dbPost = _fixture.CreateDbContext(true)
            .Posts.AsNoTracking()
            .ToList();

        dbPost.Should().HaveCount(0);
    }
}
