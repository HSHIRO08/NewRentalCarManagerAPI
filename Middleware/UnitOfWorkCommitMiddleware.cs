using NewRentalCarManagerAPI.Models;

namespace NewRentalCarManagerAPI.Middleware;

public class UnitOfWorkCommitMiddleware
{
    private readonly RequestDelegate _next;

    public UnitOfWorkCommitMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        await _next(context);

        if (!ShouldCommit(context))
        {
            return;
        }

        var dbContext = context.RequestServices.GetService<AppDbContext>();
        if (dbContext is null)
        {
            return;
        }

        await dbContext.SaveChangesAsync();
    }

    private static bool ShouldCommit(HttpContext context)
    {
        var method = context.Request.Method;
        var isWriteMethod =
            HttpMethods.IsPost(method) ||
            HttpMethods.IsPut(method) ||
            HttpMethods.IsPatch(method) ||
            HttpMethods.IsDelete(method);

        if (!isWriteMethod)
        {
            return false;
        }

        return context.Response.StatusCode is >= 200 and < 400;
    }
}
