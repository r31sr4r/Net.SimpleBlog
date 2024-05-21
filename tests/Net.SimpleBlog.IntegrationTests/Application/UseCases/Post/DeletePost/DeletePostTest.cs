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
        var input = new UseCase.DeletePostInput(postExample.Id);

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
        var input = new UseCase.DeletePostInput(Guid.NewGuid());
        var useCase = new UseCase.DeletePost(postRepository, unitOfWork);

        var task = async () => await useCase.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Post with id {input.Id} not found");
    }
}
