using FluentAssertions;
using Moq;
using Net.SimpleBlog.Application.Exceptions;
using Net.SimpleBlog.Application.UseCases.Post.Common;
using Net.SimpleBlog.Domain.Exceptions;
using DomainEntity = Net.SimpleBlog.Domain.Entity;
using UseCases = Net.SimpleBlog.Application.UseCases.Post.UpdatePost;
using Xunit;
using Net.SimpleBlog.Application.UseCases.Post.UpdatePost;

namespace Net.SimpleBlog.UnitTests.Application.Post.UpdatePost;

[Collection(nameof(UpdatePostTestFixture))]
public class UpdatePostTest
{
    private readonly UpdatePostTestFixture _fixture;

    public UpdatePostTest(UpdatePostTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Theory(DisplayName = nameof(UpdatePost))]
    [Trait("Application", "UpdatePost - Use Cases")]
    [MemberData(
        nameof(UpdatePostTestDataGenerator.GetPostsToUpdate),
        parameters: 10,
        MemberType = typeof(UpdatePostTestDataGenerator)
    )]
    public async Task UpdatePost(
        DomainEntity.Post postExample,
        UpdatePostInput input
    )
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        repositoryMock.Setup(repository => repository.Get(
                        postExample.Id,
                        It.IsAny<CancellationToken>())
               ).ReturnsAsync(postExample);
        var useCase = new UseCases.UpdatePost
            (repositoryMock.Object,
                       unitOfWorkMock.Object
                              );

        PostModelOutput output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Title.Should().Be(input.Title);
        output.Content.Should().Be(input.Content);

        repositoryMock.Verify(
            repository => repository.Get(
                postExample.Id,
                It.IsAny<CancellationToken>()
                ), Times.Once
        );
        repositoryMock.Verify(
            repository => repository.Update(
                postExample,
                It.IsAny<CancellationToken>()
                ), Times.Once
        );
        unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.Commit(
                It.IsAny<CancellationToken>()
                ), Times.Once
         );
    }

    [Fact(DisplayName = nameof(ThrowWhenPostNotFound))]
    [Trait("Application", "UpdatePost - Use Cases")]
    public async Task ThrowWhenPostNotFound()
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var input = _fixture.GetValidInput();
        repositoryMock.Setup(repository => repository.Get(
            input.Id,
            It.IsAny<CancellationToken>())
            ).ThrowsAsync(new NotFoundException($"Post '{input.Id}' not found"));
        var useCase = new UseCases.UpdatePost(
            repositoryMock.Object,
            unitOfWorkMock.Object
        );

        var task = async ()
            => await useCase.Handle(input, CancellationToken.None);

        await task.Should().ThrowAsync<NotFoundException>();

        repositoryMock.Verify(
            repository => repository.Get(
                input.Id,
                It.IsAny<CancellationToken>()
                ), Times.Once
        );
    }

    [Theory(DisplayName = nameof(ThrowWhenCantUpdatePost))]
    [Trait("Application", "UpdatePost - Use Cases")]
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
        var post = _fixture.GetValidPost();
        input.Id = post.Id;
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        repositoryMock.Setup(repository => repository.Get(
                post.Id,
                It.IsAny<CancellationToken>())
        ).ReturnsAsync(post);
        var useCase = new UseCases.UpdatePost
            (repositoryMock.Object,
            unitOfWorkMock.Object);

        var task = async ()
            => await useCase.Handle(input, CancellationToken.None);

        await task.Should()
            .ThrowAsync<EntityValidationException>()
            .WithMessage(expectedMessage);

        repositoryMock.Verify(
            repository => repository.Get(
                post.Id,
                It.IsAny<CancellationToken>()
                ), Times.Once
        );
    }
}
