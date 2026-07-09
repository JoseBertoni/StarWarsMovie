using System.Net;
using System.Text.Json;
using StarWarsMovies.Application.Common.Exceptions;

namespace StarWarsMovies.Api.Middlewares;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (ValidationException exception)
        {
            await WriteErrorResponseAsync(
                context,
                HttpStatusCode.BadRequest,
                "Validation error",
                exception.Message,
                exception.Errors);
        }
        catch (NotFoundException exception)
        {
            await WriteErrorResponseAsync(
                context,
                HttpStatusCode.NotFound,
                "Resource not found",
                exception.Message);
        }
        catch (ConflictException exception)
        {
            await WriteErrorResponseAsync(
                context,
                HttpStatusCode.Conflict,
                "Conflict",
                exception.Message);
        }
        catch (UnauthorizedException exception)
        {
            await WriteErrorResponseAsync(
                context,
                HttpStatusCode.Unauthorized,
                "Unauthorized",
                exception.Message);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "An unhandled exception occurred.");

            await WriteErrorResponseAsync(
                context,
                HttpStatusCode.InternalServerError,
                "Internal server error",
                "An unexpected error occurred.");
        }
    }

    private static async Task WriteErrorResponseAsync(
        HttpContext context,
        HttpStatusCode statusCode,
        string title,
        string detail,
        IReadOnlyDictionary<string, string[]>? errors = null)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            status = (int)statusCode,
            title,
            detail,
            errors,
            traceId = context.TraceIdentifier
        };

        var json = JsonSerializer.Serialize(
            response,
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

        await context.Response.WriteAsync(json);
    }
}