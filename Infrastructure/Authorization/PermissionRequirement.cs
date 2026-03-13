using Microsoft.AspNetCore.Authorization;

namespace NewRentalCarManagerAPI.Infrastructure.Authorization;

/// <summary>
/// Requirement that checks whether the current user's role
/// has a specific Permission (Resource + Action) in the database.
/// </summary>
public class PermissionRequirement : IAuthorizationRequirement
{
    public string Resource { get; }
    public string Action { get; }

    public PermissionRequirement(string resource, string action)
    {
        Resource = resource;
        Action = action;
    }
}
