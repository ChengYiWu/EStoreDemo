using Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Infrastructure;

/// <summary>
///  錯誤處理
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the request.");

            switch (ex)
            {
                case ValidationException e:
                    await HandleValidationException(context, e);
                    break;
                case NotFoundException e:
                    await HandleNotFoundException(context, e);
                    break;
                case FailureException e:
                    await HandleFailureException(context, e);
                    break;
                case InternalException:
                default:
                    await HandleDefaultException(context, ex);
                    break;
            }
        }
    }

    private static Task HandleValidationException(HttpContext context, ValidationException ex)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;

        return context.Response.WriteAsJsonAsync(new ValidationProblemDetails(ex.Errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Type = "Validation error"
        });
    }

    private static Task HandleNotFoundException(HttpContext context, NotFoundException ex)
    {
        context.Response.StatusCode = StatusCodes.Status404NotFound;

        return context.Response.WriteAsJsonAsync(new ProblemDetails()
        {
            Status = StatusCodes.Status404NotFound,
            Type = "Not Found",
            Title = "The specified resource was not found.",
            Detail = ex.Message
        });
    }

    private static Task HandleFailureException(HttpContext context, FailureException ex)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;

        return context.Response.WriteAsJsonAsync(new ProblemDetails()
        {
            Status = StatusCodes.Status400BadRequest,
            Type = "Operation Failure",
            Title = "Operation Failure.",
            Detail = ex.Message
        });
    }

    private static Task HandleDefaultException(HttpContext context, Exception ex)
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        // TODO，若是正式環境，應該要隱藏錯誤訊息
        return context.Response.WriteAsJsonAsync(new ProblemDetails()
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "An error occurred while processing your request.",
            Detail = ex.Message,
        });
    }
}

