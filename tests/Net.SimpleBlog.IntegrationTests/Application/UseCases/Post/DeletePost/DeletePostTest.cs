using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Net.SimpleBlog.Application.Exceptions;
using Net.SimpleBlog.Infra.Data.EF;
using Net.SimpleBlog.Infra.Data.EF.Repositories;
using UseCase = Net.SimpleBlog.Application.UseCases.Post.DeletePost;

namespace Net.SimpleBlog.IntegrationTests.Application.UseCases.Post.DeletePost;

[Collection(nameof(DeletePostTestFixture))]
public class DeletePostTest
{
    private readonly DeletePostTestFixture _fixture;

    public DeletePostTest(DeletePostTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(DeletePost))]
    [Trait("Integration/Application", "DeletePost - Use Cases")]
    public async Task DeletePost()
    {
        var dbContext = _fixture.CreateDbContext();
        var repository = new PostRepository(dbContext);
        var unitOfWork = new UnitOfWork(
            dbContext
        );

        var postExample = _fixture.GetValidPost();
        await dbContext.AddRangeAsync(_fixture.GetPostsList());
        var tracking = await dbContext.AddAsync(postExample);
        dbContext.SaveChanges();
        tracking.State = EntityState.Detached;
        var useCase = new UseCase.DeletePost(repository, unitOfWork);
        var input = new UseCase.DeletePostInput(postExample.Id, postExample.UserId);

        await useCase.Handle(input, CancellationToken.None);

        var dbPost = await (_fixture.CreateDbContext(true))
            .Posts
            .FindAsync(postExample.Id);

        dbPost.Should().BeNull();
    }

    [Fact(DisplayName = nameof(ThrowWhenPostNotFound))]
    [Trait("Integration/Application", "DeletePost - Use Cases")]
    public async Task ThrowWhenPostNotFound()
    {
        var dbContext = _fixture.CreateDbContext();
        var unitOfWork = new UnitOfWork(
            dbContext
        );
        var examplePost = _fixture.GetValidPost();
        dbContext.Add(examplePost);
        dbContext.SaveChanges();
        var postRepository = new PostRepository(dbContext);
        var input = new UseCase.DeletePostInput(Guid.NewGuid(), examplePost.UserId);
        var useCase = new UseCase.DeletePost(postRepository, unitOfWork);

        var task = async () => await useCase.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Post with id {input.Id} not found");
    }

    [Fact(DisplayName = nameof(ThrowWhenUserIsNotOwner))]
    [Trait("Integration/Application", "DeletePost - Use Cases")]
    public async Task ThrowWhenUserIsNotOwner()
    {
        var dbContext = _fixture.CreateDbContext();
        var post = _fixture.GetValidPost();
        var anotherUserId = Guid.NewGuid();

        await dbContext.AddRangeAsync(_fixture.GetPostsList());
        var trackingInfo = await dbContext.AddAsync(post);
        dbContext.SaveChanges();
        trackingInfo.State = EntityState.Detached;

        var repository = new PostRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);
        var useCase = new UseCase.DeletePost(repository, unitOfWork);

        var input = new UseCase.DeletePostInput(post.Id, anotherUserId);

        Func<Task> task = async () => await useCase.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("You are not the owner of this post.");

        var dbPost = await dbContext.Posts.FindAsync(post.Id);
        dbPost.Should().NotBeNull();
        dbPost!.Title.Should().Be(post.Title);
        dbPost.Content.Should().Be(post.Content);
        dbPost.UserId.Should().Be(post.UserId);
    }
}
