using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using NewRentalCarManagerAPI.Common;

namespace NewRentalCarManagerAPI.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Bad request");
            await WriteErrorAsync(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogWarning(ex, "Unauthorized");
            await WriteErrorAsync(context, HttpStatusCode.Unauthorized, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Conflict");
            await WriteErrorAsync(context, HttpStatusCode.Conflict, ex.Message);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogWarning(ex, "Database constraint violation");
            var message = ex.InnerException?.Message ?? ex.Message;
            await WriteErrorAsync(context, HttpStatusCode.Conflict, message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            var message = _env.IsDevelopment()
                ? ex.InnerException?.Message ?? ex.Message
                : "An unexpected error occurred";
            await WriteErrorAsync(context, HttpStatusCode.InternalServerError, message);
        }
    }

    private static async Task WriteErrorAsync(HttpContext context, HttpStatusCode code, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;
        var result = ApiResult<object>.Fail(message);
        var json = JsonSerializer.Serialize(result, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        await context.Response.WriteAsync(json);
    }
}
