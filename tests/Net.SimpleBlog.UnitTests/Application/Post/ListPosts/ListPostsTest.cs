using FluentAssertions;
using Moq;
using Net.SimpleBlog.Application.UseCases.Post.Common;
using Net.SimpleBlog.Domain.SeedWork.SearchableRepository;
using Xunit;
using DomainEntity = Net.SimpleBlog.Domain.Entity;
using UseCase = Net.SimpleBlog.Application.UseCases.Post.ListPosts;

namespace Net.SimpleBlog.UnitTests.Application.Post.ListPosts;

[Collection(nameof(ListPostsTestFixture))]
public class ListPostsTest
{
    private readonly ListPostsTestFixture _fixture;

    public ListPostsTest(ListPostsTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact(DisplayName = nameof(ShouldReturnPosts))]
    [Trait("Application", "ListPosts - Use Cases")]
    public async Task ShouldReturnPosts()
    {
        var postsList = _fixture.GetPostsList();
        var repositoryMock = _fixture.GetRepositoryMock();
        var input = _fixture.GetInput();
        var outputRepositorySearch = new SearchOutput<DomainEntity.Post>(
            currentPage: input.Page,
            perPage: input.PerPage,
            items: (IReadOnlyList<DomainEntity.Post>)postsList,
            total: new Random().Next(50, 200)
        );
        repositoryMock.Setup(x => x.Search(
            It.Is<SearchInput>(
                searchInput => searchInput.Page == input.Page &&
                searchInput.PerPage == input.PerPage &&
                searchInput.Search == input.Search &&
                searchInput.OrderBy == input.Sort &&
                searchInput.Order == input.Dir
            ),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(outputRepositorySearch);

        var useCase = new UseCase.ListPosts(repositoryMock.Object);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(outputRepositorySearch.CurrentPage);
        output.PerPage.Should().Be(outputRepositorySearch.PerPage);
        output.Total.Should().Be(outputRepositorySearch.Total);
        output.Items.Should().HaveCount(outputRepositorySearch.Items.Count);
        ((List<PostModelOutput>)output.Items).ForEach(outputItem =>
        {
            var repositoryPost = outputRepositorySearch.Items
                .FirstOrDefault(p => p.Id == outputItem.Id);
            outputItem.Should().NotBeNull();
            outputItem.Title.Should().Be(repositoryPost!.Title);
            outputItem.Content.Should().Be(repositoryPost!.Content);
            outputItem.UserId.Should().Be(repositoryPost!.UserId);
            outputItem.Id.Should().Be(repositoryPost!.Id);
        });
        repositoryMock.Verify(x => x.Search(
            It.Is<SearchInput>(
                searchInput => searchInput.Page == input.Page &&
                searchInput.PerPage == input.PerPage &&
                searchInput.Search == input.Search &&
                searchInput.OrderBy == input.Sort &&
                searchInput.Order == input.Dir
            ),
            It.IsAny<CancellationToken>()
            ), Times.Once);
    }

    [Theory(DisplayName = nameof(ListInputWithoutAllParameters))]
    [Trait("Application", "ListPosts - Use Cases")]
    [MemberData(nameof(ListPostsTestDataGenerator.GetInputWithoutAllParameters),
        parameters: 18,
        MemberType = typeof(ListPostsTestDataGenerator)
        )]
    public async Task ListInputWithoutAllParameters(
        UseCase.ListPostsInput input
        )
    {
        var postsList = _fixture.GetPostsList();
        var repositoryMock = _fixture.GetRepositoryMock();
        var outputRepositorySearch = new SearchOutput<DomainEntity.Post>(
            currentPage: input.Page,
            perPage: input.PerPage,
            items: (IReadOnlyList<DomainEntity.Post>)postsList,
            total: new Random().Next(50, 200)
        );
        repositoryMock.Setup(x => x.Search(
            It.Is<SearchInput>(
                searchInput => searchInput.Page == input.Page &&
                searchInput.PerPage == input.PerPage &&
                searchInput.Search == input.Search &&
                searchInput.OrderBy == input.Sort &&
                searchInput.Order == input.Dir
            ),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(outputRepositorySearch);

        var useCase = new UseCase.ListPosts(repositoryMock.Object);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(outputRepositorySearch.CurrentPage);
        output.PerPage.Should().Be(outputRepositorySearch.PerPage);
        output.Total.Should().Be(outputRepositorySearch.Total);
        output.Items.Should().HaveCount(outputRepositorySearch.Items.Count);
        ((List<PostModelOutput>)output.Items).ForEach(outputItem =>
        {
            var repositoryPost = outputRepositorySearch.Items
                .FirstOrDefault(p => p.Id == outputItem.Id);
            outputItem.Should().NotBeNull();
            outputItem.Title.Should().Be(repositoryPost!.Title);
            outputItem.Content.Should().Be(repositoryPost!.Content);
            outputItem.UserId.Should().Be(repositoryPost!.UserId);
            outputItem.Id.Should().Be(repositoryPost!.Id);
        });
        repositoryMock.Verify(x => x.Search(
            It.Is<SearchInput>(
                searchInput => searchInput.Page == input.Page &&
                searchInput.PerPage == input.PerPage &&
                searchInput.Search == input.Search &&
                searchInput.OrderBy == input.Sort &&
                searchInput.Order == input.Dir
            ),
            It.IsAny<CancellationToken>()
            ), Times.Once);
    }

    [Fact(DisplayName = nameof(ListOkWhenEmpty))]
    [Trait("Application", "ListPosts - Use Cases")]
    public async Task ListOkWhenEmpty()
    {
        var input = _fixture.GetInput();
        var repositoryMock = _fixture.GetRepositoryMock();
        var outputRepositorySearch = new SearchOutput<DomainEntity.Post>(
            currentPage: input.Page,
            perPage: input.PerPage,
            items: new List<DomainEntity.Post>().AsReadOnly(),
            total: 0
        );
        repositoryMock.Setup(x => x.Search(
            It.Is<SearchInput>(
                searchInput => searchInput.Page == input.Page &&
                searchInput.PerPage == input.PerPage &&
                searchInput.Search == input.Search &&
                searchInput.OrderBy == input.Sort &&
                searchInput.Order == input.Dir
            ),
            It.IsAny<CancellationToken>()
        )).ReturnsAsync(outputRepositorySearch);

        var useCase = new UseCase.ListPosts(repositoryMock.Object);

        var output = await useCase.Handle(input, CancellationToken.None);

        output.Should().NotBeNull();
        output.Page.Should().Be(outputRepositorySearch.CurrentPage);
        output.PerPage.Should().Be(outputRepositorySearch.PerPage);
        output.Total.Should().Be(0);
        output.Items.Should().HaveCount(0);

        repositoryMock.Verify(x => x.Search(
            It.Is<SearchInput>(
                searchInput => searchInput.Page == input.Page &&
                searchInput.PerPage == input.PerPage &&
                searchInput.Search == input.Search &&
                searchInput.OrderBy == input.Sort &&
                searchInput.Order == input.Dir
            ),
            It.IsAny<CancellationToken>()
            ), Times.Once);
    }
}
