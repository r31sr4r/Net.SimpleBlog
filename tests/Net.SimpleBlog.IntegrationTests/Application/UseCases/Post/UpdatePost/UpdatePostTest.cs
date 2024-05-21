using Net.SimpleBlog.Application.UseCases.Post.UpdatePost;
using Net.SimpleBlog.Infra.Data.EF.Repositories;
using Net.SimpleBlog.Infra.Data.EF;
using DomainEntity = Net.SimpleBlog.Domain.Entity;
using UseCase = Net.SimpleBlog.Application.UseCases.Post.UpdatePost;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Net.SimpleBlog.Application.Exceptions;
using Net.SimpleBlog.Domain.Exceptions;


namespace Net.SimpleBlog.IntegrationTests.Application.UseCases.Post.UpdatePost;

[Collection(nameof(UpdatePostTestFixture))]
public class UpdatePostTest
{
    private readonly UpdatePostTestFixture _fixture;

    public UpdatePostTest(UpdatePostTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Theory(DisplayName = nameof(UpdatePost))]
    [Trait("Integration/Application", "UpdatePost - Use Cases")]
    [MemberData(
        nameof(UpdatePostTestDataGenerator.GetPostsToUpdate),
        parameters: 5,
        MemberType = typeof(UpdatePostTestDataGenerator)
    )]
    public async Task UpdatePost(
        DomainEntity.Post postExample,
        UpdatePostInput input
    )
    {
        var dbContext = _fixture.CreateDbContext();
        await dbContext.AddRangeAsync(_fixture.GetPostsList());
        var trackingInfo = await dbContext.AddAsync(postExample);
        dbContext.SaveChanges();
        trackingInfo.State = EntityState.Detached;
        var repository = new PostRepository(dbContext);
        var unitOfWork = new UnitOfWork(
            dbContext
        );
        var useCase = new UseCase.UpdatePost(repository, unitOfWork);

        var output = await useCase.Handle(input, CancellationToken.None);

        var dbPost = await (_fixture.CreateDbContext(true))
            .Posts.FindAsync(output.Id);

        dbPost.Should().NotBeNull();
        dbPost!.Title.Should().Be(input.Title);
        dbPost.Content.Should().Be(input.Content);
        dbPost.UserId.Should().Be(postExample.UserId);
        dbPost.UpdatedAt.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(100));

        output.Should().NotBeNull();
        output.Title.Should().Be(input.Title);
        output.Content.Should().Be(input.Content);
        output.UserId.Should().Be(postExample.UserId);
        output.Id.Should().Be(postExample.Id);
    }

    [Fact(DisplayName = nameof(ThrowWhenPostNotFound))]
    [Trait("Integration/Application", "UpdatePost - Use Cases")]
    public async Task ThrowWhenPostNotFound()
    {
        var input = _fixture.GetValidInput();
        var dbContext = _fixture.CreateDbContext();
        await dbContext.AddRangeAsync(_fixture.GetPostsList());
        dbContext.SaveChanges();
        var repository = new PostRepository(dbContext);
        var unitOfWork = new UnitOfWork(
            dbContext
        );
        var useCase = new UseCase.UpdatePost(repository, unitOfWork);

        var task = async ()
            => await useCase.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<NotFoundException>()
            .WithMessage($"Post with id {input.Id} not found");
    }

    [Theory(DisplayName = nameof(ThrowWhenCantUpdatePost))]
    [Trait("Integration/Application", "UpdatePost - Use Cases")]
    [MemberData(
        nameof(UpdatePostTestDataGenerator.GetInvalidInputs),
        parameters: 12,
        MemberType = typeof(UpdatePostTestDataGenerator)
    )]
    public async Task ThrowWhenCantUpdatePost(
    UpdatePostInput input,
    string expectedMessage
    )
    {
        var dbContext = _fixture.CreateDbContext();
        var examplePosts = _fixture.GetPostsList();
        await dbContext.AddRangeAsync(examplePosts);
        dbContext.SaveChanges();
        var repository = new PostRepository(dbContext);
        var unitOfWork = new UnitOfWork(
            dbContext
        );
        var useCase = new UseCase.UpdatePost(repository, unitOfWork);
        input.Id = examplePosts[0].Id;

        var task = async ()
            => await useCase.Handle(input, CancellationToken.None);

        await task.Should()
            .ThrowAsync<EntityValidationException>()
            .WithMessage(expectedMessage);
    }
}

