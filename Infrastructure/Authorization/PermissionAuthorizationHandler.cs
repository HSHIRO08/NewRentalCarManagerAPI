using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using NewRentalCarManagerAPI.Domain.Interfaces;
using NewRentalCarManagerAPI.Models;

namespace NewRentalCarManagerAPI.Infrastructure.Authorization;

/// <summary>
/// Authorization handler that checks if the current user's role has the required permission.
/// </summary>
public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<PermissionAuthorizationHandler> _logger;

    public PermissionAuthorizationHandler(IUnitOfWork uow, ILogger<PermissionAuthorizationHandler> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        // Get the user ID from claims
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)
            ?? context.User.FindFirst("sub");

        if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out var userId))
        {
            _logger.LogWarning("User ID not found in claims");
            context.Fail();
            return;
        }

        try
        {
            // Get the user with their role and role's permissions
            var user = await _uow.Users.Query()
                .Include(u => u.Role)
                .ThenInclude(r => r.Permissions)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found", userId);
                context.Fail();
                return;
            }

            // Check if the user's role has the required permission
            var hasPermission = user.Role.Permissions.Any(p =>
                p.Resource.Equals(requirement.Resource, StringComparison.OrdinalIgnoreCase) &&
                p.Action.Equals(requirement.Action, StringComparison.OrdinalIgnoreCase));

            if (hasPermission)
            {
                context.Succeed(requirement);
                _logger.LogInformation(
                    "User {UserId} authorized for {Resource}:{Action}",
                    userId, requirement.Resource, requirement.Action);
            }
            else
            {
                _logger.LogWarning(
                    "User {UserId} denied access to {Resource}:{Action}",
                    userId, requirement.Resource, requirement.Action);
                context.Fail();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking permission {Resource}:{Action}", 
                requirement.Resource, requirement.Action);
            context.Fail();
        }
    }
}
