using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace NewRentalCarManagerAPI.Infrastructure.Authorization;

/// <summary>
/// Attribute-based permission check, e.g. [HasPermission("cars", "create")].
/// Creates a dynamic PermissionRequirement and evaluates it via IAuthorizationService.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class HasPermissionAttribute : Attribute, IAsyncAuthorizationFilter
{
    private readonly string _resource;
    private readonly string _action;

    public HasPermissionAttribute(string resource, string action)
    {
        _resource = resource;
        _action = action;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var authService = context.HttpContext.RequestServices
            .GetRequiredService<IAuthorizationService>();

        var requirement = new PermissionRequirement(_resource, _action);
        var result = await authService.AuthorizeAsync(
            context.HttpContext.User, null, requirement);

        if (!result.Succeeded)
        {
            context.Result = new ForbidResult();
        }
    }
}
