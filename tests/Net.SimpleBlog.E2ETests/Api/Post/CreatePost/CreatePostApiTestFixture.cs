using Net.SimpleBlog.Application.UseCases.Post.CreatePost;
using Net.SimpleBlog.Domain.Entity;
using Net.SimpleBlog.E2ETests.Api.Post.Common;
using Net.SimpleBlog.E2ETests.Base;
using Microsoft.EntityFrameworkCore;
using Net.SimpleBlog.Infra.Data.EF;

[CollectionDefinition(nameof(CreatePostApiTestFixture))]
public class CreatePostApiTestFixtureCollection
    : ICollectionFixture<CreatePostApiTestFixture>
{ }

public class CreatePostApiTestFixture : PostBaseFixture
{
    
    public CreatePostInput GetInput()
    {
        var post = GetValidPost(AuthenticatedUser.Id);
        return new CreatePostInput(
            post.Title,
            post.Content,
            AuthenticatedUser.Id
        );
    }
}
