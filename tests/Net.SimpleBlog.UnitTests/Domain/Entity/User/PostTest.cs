using System;
using Xunit;
using FluentAssertions;
using Net.SimpleBlog.Domain.Exceptions;
using DomainEntity = Net.SimpleBlog.Domain.Entity;

namespace Net.SimpleBlog.UnitTests.Domain.Entity.Post
{
    public class PostTest
    {
        private class PostData
        {
            public string? Title { get; set; }
            public string? Content { get; set; }
            public Guid UserId { get; set; }
        }

        private PostData GetInitialData() => new PostData
        {
            Title = "Sample Title",
            Content = "Sample Content",
            UserId = Guid.NewGuid()
        };

        private DomainEntity.Post CreatePost(PostData data) =>
            new DomainEntity.Post(data.Title!, data.Content!, data.UserId);

        [Fact(DisplayName = nameof(Instantiate))]
        [Trait("Domain", "Post - Aggregates")]
        public void Instantiate()
        {
            var validData = new
            {
                Title = "Sample Title",
                Content = "Sample Content",
                UserId = Guid.NewGuid()
            };
            var dateTimeBefore = DateTime.Now;

            var post = new DomainEntity.Post(
                validData.Title,
                validData.Content,
                validData.UserId
            );
            var dateTimeAfter = DateTime.Now;

            post.Should().NotBeNull();
            post.Title.Should().Be(validData.Title);
            post.Content.Should().Be(validData.Content);
            post.UserId.Should().Be(validData.UserId);
            post.CreatedAt.Should().NotBe(default);
            post.CreatedAt.Should().BeAfter(dateTimeBefore).And.BeBefore(dateTimeAfter);
        }

        [Theory(DisplayName = nameof(InstantiateErrorWhenTitleIsEmpty))]
        [Trait("Domain", "Post - Aggregates")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("  ")]
        public void InstantiateErrorWhenTitleIsEmpty(string? title)
        {
            var data = new
            {
                Title = title,
                Content = "Sample Content",
                UserId = Guid.NewGuid()
            };

            Action action = () => new DomainEntity.Post(
                data.Title!,
                data.Content,
                data.UserId
            );

            action.Should()
                .Throw<EntityValidationException>()
                .WithMessage("Title should not be empty or null");
        }

        [Theory(DisplayName = nameof(InstantiateErrorWhenContentIsEmpty))]
        [Trait("Domain", "Post - Aggregates")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("  ")]
        public void InstantiateErrorWhenContentIsEmpty(string? content)
        {
            var data = new
            {
                Title = "Sample Title",
                Content = content,
                UserId = Guid.NewGuid()
            };

            Action action = () => new DomainEntity.Post(
                data.Title,
                data.Content!,
                data.UserId
            );

            action.Should()
                .Throw<EntityValidationException>()
                .WithMessage("Content should not be empty or null");
        }

        [Theory(DisplayName = nameof(InstantiateErrorWhenTitleIsLessThan4Characters))]
        [Trait("Domain", "Post - Aggregates")]
        [InlineData("abc")]
        [InlineData("a")]
        public void InstantiateErrorWhenTitleIsLessThan4Characters(string invalidTitle)
        {
            var data = new
            {
                Title = invalidTitle,
                Content = "Sample Content",
                UserId = Guid.NewGuid()
            };

            Action action = () => new DomainEntity.Post(
                data.Title,
                data.Content,
                data.UserId
            );

            action.Should()
                .Throw<EntityValidationException>()
                .WithMessage("Title should be greater than 3 characters");
        }

        [Fact(DisplayName = nameof(InstantiateErrorWhenTitleIsGreaterThan255Characters))]
        [Trait("Domain", "Post - Aggregates")]
        public void InstantiateErrorWhenTitleIsGreaterThan255Characters()
        {
            var invalidTitle = new string('a', 256);

            Action action = () => new DomainEntity.Post(
                invalidTitle,
                "Sample Content",
                Guid.NewGuid()
            );

            action.Should()
                .Throw<EntityValidationException>()
                .WithMessage("Title should be less than 255 characters");
        }

        [Fact(DisplayName = nameof(Update))]
        [Trait("Domain", "Post - Aggregates")]
        public void Update()
        {
            var initialData = new
            {
                Title = "Sample Title",
                Content = "Sample Content",
                UserId = Guid.NewGuid()
            };
            var post = new DomainEntity.Post(
                initialData.Title,
                initialData.Content,
                initialData.UserId
            );
            var updatedData = new
            {
                Title = "Updated Title",
                Content = "Updated Content"
            };

            post.Update(
                updatedData.Title,
                updatedData.Content
            );

            post.Title.Should().Be(updatedData.Title);
            post.Content.Should().Be(updatedData.Content);
            post.UpdatedAt.Should().NotBeNull();
        }

        [Theory(DisplayName = nameof(UpdateErrorWhenTitleIsEmpty))]
        [Trait("Domain", "Post - Aggregates")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("  ")]
        public void UpdateErrorWhenTitleIsEmpty(string? title)
        {
            var initialData = GetInitialData();
            var post = CreatePost(initialData);

            Action action = () => post.Update(title!, initialData.Content);

            action.Should()
                .Throw<EntityValidationException>()
                .WithMessage("Title should not be empty or null");
        }

        [Theory(DisplayName = nameof(UpdateErrorWhenContentIsEmpty))]
        [Trait("Domain", "Post - Aggregates")]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("  ")]
        public void UpdateErrorWhenContentIsEmpty(string? content)
        {
            var initialData = GetInitialData();
            var post = CreatePost(initialData);

            Action action = () => post.Update(initialData.Title, content!);

            action.Should()
                .Throw<EntityValidationException>()
                .WithMessage("Content should not be empty or null");
        }

        [Theory(DisplayName = nameof(UpdateErrorWhenTitleIsLessThan4Characters))]
        [Trait("Domain", "Post - Aggregates")]
        [InlineData("abc")]
        [InlineData("a")]
        public void UpdateErrorWhenTitleIsLessThan4Characters(string invalidTitle)
        {
            var initialData = GetInitialData();
            var post = CreatePost(initialData);

            Action action = () => post.Update(invalidTitle, initialData.Content);

            action.Should()
                .Throw<EntityValidationException>()
                .WithMessage("Title should be greater than 3 characters");
        }

        [Fact(DisplayName = nameof(UpdateErrorWhenTitleIsGreaterThan255Characters))]
        [Trait("Domain", "Post - Aggregates")]
        public void UpdateErrorWhenTitleIsGreaterThan255Characters()
        {
            var invalidTitle = new string('a', 256);
            var initialData = GetInitialData();
            var post = CreatePost(initialData);

            Action action = () => post.Update(invalidTitle, initialData.Content);

            action.Should()
                .Throw<EntityValidationException>()
                .WithMessage("Title should be less than 255 characters");
        }
    }
}
