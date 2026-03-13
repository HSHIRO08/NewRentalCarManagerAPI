using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using NewRentalCarManagerAPI.Models;

namespace NewRentalCarManagerAPI.Infrastructure.Authorization;

/// <summary>
/// Checks whether the authenticated user's role has the required
/// Permission (Resource + Action) via the role_permissions join table.
/// </summary>
public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IServiceScopeFactory _scopeFactory;

    public PermissionHandler(IServiceScopeFactory scopeFactory)
        => _scopeFactory = scopeFactory;

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)
                          ?? context.User.FindFirst("sub");

        if (userIdClaim is null || !Guid.TryParse(userIdClaim.Value, out var userId))
            return;

        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var hasPermission = await db.Users
            .Where(u => u.Id == userId)
            .SelectMany(u => u.Role.Permissions)
            .AnyAsync(p => p.Resource == requirement.Resource
                        && p.Action == requirement.Action);

        if (hasPermission)
            context.Succeed(requirement);
    }
}
