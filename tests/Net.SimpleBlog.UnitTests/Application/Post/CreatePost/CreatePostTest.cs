using Moq;
using FluentAssertions;
using Net.SimpleBlog.Application.UseCases.Post.CreatePost;
using Net.SimpleBlog.Domain.Exceptions;
using DomainEntity = Net.SimpleBlog.Domain.Entity;
using Xunit;
using UseCases = Net.SimpleBlog.Application.UseCases.Post.CreatePost;


namespace Net.SimpleBlog.UnitTests.Application.Post.CreatePost;

[Collection(nameof(CreatePostTestFixture))]
public class CreatePostTest
{
    private readonly CreatePostTestFixture _fixture;

    public CreatePostTest(CreatePostTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(CreatePost))]
    [Trait("Application", "Create Post - Use Cases")]
    public async void CreatePost()
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();

        var useCase = new UseCases.CreatePost(
            repositoryMock.Object,
            unitOfWorkMock.Object
        );

        var input = _fixture.GetInput();

        var output = await useCase.Handle(input, CancellationToken.None);

        repositoryMock.Verify(
            repository => repository.Insert(
                It.IsAny<DomainEntity.Post>(),
                It.IsAny<CancellationToken>()
            ),
            Times.Once
        );

        unitOfWorkMock.Verify(
            unitOfWork => unitOfWork.Commit(It.IsAny<CancellationToken>()),
            Times.Once
        );

        output.Should().NotBeNull();
        output.Title.Should().Be(input.Title);
        output.Content.Should().Be(input.Content);
        output.UserId.Should().Be(input.UserId);
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBeSameDateAs(default);
    }

    [Theory(DisplayName = nameof(ThrowWhenCantInstantiatePost))]
    [Trait("Application", "Create Post - Use Cases")]
    [MemberData(
        nameof(CreatePostTestDataGenerator.GetInvalidInputs),
        parameters: 6,
        MemberType = typeof(CreatePostTestDataGenerator)
    )]
    public async void ThrowWhenCantInstantiatePost(
        CreatePostInput input,
        string expectionMessage
    )
    {
        var useCase = new UseCases.CreatePost(
            _fixture.GetRepositoryMock().Object,
            _fixture.GetUnitOfWorkMock().Object
        );

        Func<Task> task = async () => await useCase.Handle(input, CancellationToken.None);

        await task.Should()
            .ThrowAsync<EntityValidationException>()
            .WithMessage(expectionMessage);
    }
}
