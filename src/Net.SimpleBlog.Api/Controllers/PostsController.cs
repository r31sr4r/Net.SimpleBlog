using Net.SimpleBlog.Api.ApiModels.Response;
using Net.SimpleBlog.Api.ApiModels.Post;
using Net.SimpleBlog.Api.WebSockets;
using Net.SimpleBlog.Application.UseCases.Post.Common;
using Net.SimpleBlog.Application.UseCases.Post.CreatePost;
using Net.SimpleBlog.Application.UseCases.Post.DeletePost;
using Net.SimpleBlog.Application.UseCases.Post.GetPost;
using Net.SimpleBlog.Application.UseCases.Post.ListPosts;
using Net.SimpleBlog.Application.UseCases.Post.UpdatePost;
using Net.SimpleBlog.Domain.SeedWork.SearchableRepository;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Net.SimpleBlog.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class PostsController : ControllerBase
{
    private readonly ILogger<PostsController> _logger;
    private readonly IMediator _mediator;
    private readonly WebSocketConnectionManager _webSocketManager;

    public PostsController(
        ILogger<PostsController> logger,
        IMediator mediator,
        WebSocketConnectionManager webSocketManager
        )
    {
        _logger = logger;
        _mediator = mediator;
        _webSocketManager = webSocketManager;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<PostModelOutput>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(
        [FromBody] CreatePostInput input,
        CancellationToken cancellationToken
        )
    {
        var result = await _mediator.Send(input, cancellationToken);
        await _webSocketManager.BroadcastMessageAsync($"New post created: {result.Title}");

        return CreatedAtAction(
            nameof(Create),
            new { result.Id },
            new ApiResponse<PostModelOutput>(result)
        );
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PostModelOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        var result = await _mediator.Send(
            new GetPostInput(id),
            cancellationToken
        );

        return Ok(new ApiResponse<PostModelOutput>(result));
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        await _mediator.Send(
            new DeletePostInput(id),
            cancellationToken
        );

        return NoContent();
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<PostModelOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdatePostApiInput apiInput,
        CancellationToken cancellationToken
    )
    {
        var input = new UpdatePostInput(
            id,
            apiInput.Title,
            apiInput.Content
        );
        var result = await _mediator.Send(input, cancellationToken);
        return Ok(new ApiResponse<PostModelOutput>(result));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ListPostsOutput), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        CancellationToken cancellation,
        [FromQuery] int? page = null,
        [FromQuery(Name = "per_page")] int? perPage = null,
        [FromQuery] string? search = null,
        [FromQuery] string? sort = null,
        [FromQuery] SearchOrder? dir = null
    )
    {
        var input = new ListPostsInput();
        if (page.HasValue)
            input.Page = page.Value;
        if (perPage.HasValue)
            input.PerPage = perPage.Value;
        if (!string.IsNullOrWhiteSpace(search))
            input.Search = search;
        if (!string.IsNullOrWhiteSpace(sort))
            input.Sort = sort;
        if (dir.HasValue)
            input.Dir = dir.Value;

        var output = await _mediator.Send(input, cancellation);

        return Ok(new ApiResponseList<PostModelOutput>(output));
    }
}
