using Net.SimpleBlog.Application.Exceptions;
using Net.SimpleBlog.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Net.SimpleBlog.Api.Filters;

public class ApiGlobalExceptionFilter : IExceptionFilter
{
    private readonly IHostEnvironment _env;
    private readonly ILogger<ApiGlobalExceptionFilter> _logger;

    public ApiGlobalExceptionFilter(IHostEnvironment env, ILogger<ApiGlobalExceptionFilter> logger)
    {
        _env = env;
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        var details = new ProblemDetails();
        var exception = context.Exception;

        _logger.LogError(exception, "An exception occurred: {ExceptionMessage}", exception.Message);

        if (_env.IsDevelopment())
            details.Extensions["trace"] = exception.StackTrace;

        if (exception is EntityValidationException)
        {
            details.Title = "One or more validation errors occurred";
            details.Status = StatusCodes.Status422UnprocessableEntity;
            details.Type = "UnprocessableEntity";
            details.Detail = exception.Message;
        }
        else if (exception is NotFoundException)
        {
            details.Title = "Not found";
            details.Status = StatusCodes.Status404NotFound;
            details.Type = "NotFound";
            details.Detail = exception.Message;
        }
        else if (exception is CustomAuthenticationException)
        {
            details.Title = "Authentication error";
            details.Status = StatusCodes.Status401Unauthorized;
            details.Type = "Unauthorized";
            details.Detail = exception.Message;
        }
        else
        {
            details.Title = "An error occurred while processing your request";
            details.Status = StatusCodes.Status500InternalServerError;
            details.Detail = exception.Message;
        }

        context.HttpContext.Response.StatusCode = details.Status.Value;
        context.Result = new ObjectResult(details);
        context.ExceptionHandled = true;
    }
}
