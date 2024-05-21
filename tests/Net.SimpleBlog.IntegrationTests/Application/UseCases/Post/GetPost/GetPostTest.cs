using Net.SimpleBlog.Application.Exceptions;
using Net.SimpleBlog.Infra.Data.EF.Repositories;
using FluentAssertions;
using UseCase = Net.SimpleBlog.Application.UseCases.Post.GetPost;

namespace Net.SimpleBlog.IntegrationTests.Application.UseCases.Post.GetPost;

[Collection(nameof(GetPostTestFixture))]
public class GetPostTest
{
    private readonly GetPostTestFixture _fixture;

    public GetPostTest(GetPostTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(GetPost))]
    [Trait("Integration/Application", "GetPost - Use Cases")]
    public async Task GetPost()
    {
        var dbContext = _fixture.CreateDbContext();
        var examplePost = _fixture.GetValidPost();
        dbContext.Add(examplePost);
        dbContext.SaveChanges();
        var postRepository = new PostRepository(dbContext);

        var input = new UseCase.GetPostInput(examplePost.Id);
        var useCase = new UseCase.GetPost(postRepository);

        var output = await useCase.Handle(input, CancellationToken.None);

        var dbPost = await (_fixture.CreateDbContext(true))
            .Posts
            .FindAsync(examplePost.Id);

        dbPost.Should().NotBeNull();
        dbPost!.Title.Should().Be(examplePost.Title);
        dbPost.Content.Should().Be(examplePost.Content);
        dbPost.UserId.Should().Be(examplePost.UserId);
        dbPost.CreatedAt.Should().BeCloseTo(examplePost.CreatedAt, TimeSpan.FromMilliseconds(100));
        dbPost.Id.Should().Be(examplePost.Id);

        output.Should().NotBeNull();
        output!.Title.Should().Be(examplePost.Title);
        output.Content.Should().Be(examplePost.Content);
        output.UserId.Should().Be(examplePost.UserId);
        output.CreatedAt.Should().BeCloseTo(examplePost.CreatedAt, TimeSpan.FromMilliseconds(100));
        output.Id.Should().Be(examplePost.Id);
    }

    [Fact(DisplayName = nameof(NotFoundExceptionWhenPostDoesntExist))]
    [Trait("Integration/Application", "GetPost - Use Cases")]
    public async Task NotFoundExceptionWhenPostDoesntExist()
    {
        var dbContext = _fixture.CreateDbContext();
        var examplePost = _fixture.GetValidPost();
        dbContext.Add(examplePost);
        dbContext.SaveChanges();
        var postRepository = new PostRepository(dbContext);
        var input = new UseCase.GetPostInput(Guid.NewGuid());
        var useCase = new UseCase.GetPost(postRepository);

        var task = async () => await useCase.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Post with id {input.Id} not found");
    }
}
