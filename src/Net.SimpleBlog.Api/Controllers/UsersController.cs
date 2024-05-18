using Net.SimpleBlog.Api.ApiModels.Response;
using Net.SimpleBlog.Api.ApiModels.User;
using Net.SimpleBlog.Api.Common.Utilities;
using Net.SimpleBlog.Application.UseCases.User.AuthUser;
using Net.SimpleBlog.Application.UseCases.User.Common;
using Net.SimpleBlog.Application.UseCases.User.CreateUser;
using Net.SimpleBlog.Application.UseCases.User.DeleteUser;
using Net.SimpleBlog.Application.UseCases.User.GetUser;
using Net.SimpleBlog.Application.UseCases.User.ListUsers;
using Net.SimpleBlog.Application.UseCases.User.Update;
using Net.SimpleBlog.Domain.SeedWork.SearchableRepository;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Net.SimpleBlog.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly IMediator _mediator;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public UsersController(
        ILogger<UsersController> logger,
        IMediator mediator,
        IJwtTokenGenerator tokenGenerator
        )
    {
        _logger = logger;
        _mediator = mediator;
        _tokenGenerator = tokenGenerator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<UserModelOutput>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Create(
        [FromBody] CreateUserInput input,
        CancellationToken cancellationToken
        )
    {
        var result = await _mediator.Send(input, cancellationToken);
        return CreatedAtAction(
            nameof(Create), 
            new { result.Id},
            new ApiResponse<UserModelOutput>(result)
        );
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<UserModelOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        [FromRoute] Guid id,
        CancellationToken cancellationToken
    )
    {
        var result = await _mediator.Send(
            new GetUserInput(id), 
            cancellationToken
        );

        return Ok(new ApiResponse<UserModelOutput>(result));
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
            new DeleteUserInput(id), 
            cancellationToken
        );

        return NoContent();
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<UserModelOutput>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> Update(
        [FromRoute] Guid id,
        [FromBody] UpdateUserApiInput apiInput,
        CancellationToken cancellationToken
    )
    {
        var input = new UpdateUserInput(
            id,
            apiInput.Name,
            apiInput.Email,
            apiInput.Phone,
            apiInput.CPF,
            apiInput.DateOfBirth,
            apiInput.RG,
            apiInput.IsActive
        );
        var result = await _mediator.Send(input, cancellationToken);
        return Ok(new ApiResponse<UserModelOutput>(result));
    }

    [HttpGet]
    [ProducesResponseType(typeof(ListUsersOutput), StatusCodes.Status200OK)]
    public async Task<IActionResult> List(
        CancellationToken cancellation,
        [FromQuery] int? page = null,
        [FromQuery(Name = "per_page")] int? perPage = null,
        [FromQuery] string? search = null,
        [FromQuery] string? sort = null,
        [FromQuery] SearchOrder? dir = null
    )
    {
        var input = new ListUsersInput();
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

        return Ok(new ApiResponseList<UserModelOutput>(output));
    }

    [HttpPost("authenticate")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Authenticate(
        [FromBody] AuthUserInput authUserInput,
        CancellationToken cancellationToken
    )
    {
        var user = await _mediator.Send(authUserInput, cancellationToken);
        var token = _tokenGenerator.GenerateJwtToken(user);
        var response = new AuthResponse 
        {
            Email = user.Email,
            Token = token
        };
        return Ok(response);
    }

}
