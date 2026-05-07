using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewRentalCarManagerAPI.Common;
using NewRentalCarManagerAPI.Domain.Interfaces;
using NewRentalCarManagerAPI.Infrastructure.Authorization;

namespace NewRentalCarManagerAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PermissionsController : ControllerBase
{
    private readonly IUnitOfWork _uow;
    private readonly ILogger<PermissionsController> _logger;

    public PermissionsController(IUnitOfWork uow, ILogger<PermissionsController> logger)
    {
        _uow = uow;
        _logger = logger;
    }

    /// <summary>
    /// Get current user's permissions based on their role
    /// </summary>
    [HttpGet("me")]
    public async Task<IActionResult> GetMyPermissions()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("sub")?.Value;

        if (!Guid.TryParse(userIdClaim, out var userId))
            return Unauthorized(DataResult.ResultError(401, "Invalid user ID"));

        var user = await _uow.Users.GetByIdAsync(userId);
        if (user == null)
            return NotFound(DataResult.ResultError(404, "User not found"));

        var role = await _uow.Roles.Query()
            .Where(r => r.Id == user.RoleId)
            .Include(r => r.Permissions)
            .FirstOrDefaultAsync();

        if (role == null)
            return NotFound(DataResult.ResultError(404, "Role not found"));

        var permissions = role.Permissions.Select(p => new
        {
            p.Resource,
            p.Action,
            Permission = $"{p.Resource}:{p.Action}"
        }).ToList();

        return Ok(DataResult.ResultSuccess(new
        {
            UserId = userId,
            Role = role.Name,
            Permissions = permissions,
            PermissionCount = permissions.Count
        }, "Get success!"));
    }

    /// <summary>
    /// Get all available permissions (admin only)
    /// </summary>
    [HasPermission("permissions", "read")]
    [HttpGet]
    public async Task<IActionResult> GetAllPermissions()
    {
        var permissions = await _uow.Permissions.GetAllAsync();
        
        var grouped = permissions
            .GroupBy(p => p.Resource)
            .Select(g => new
            {
                Resource = g.Key,
                Actions = g.Select(p => p.Action).ToList()
            })
            .ToList();

        return Ok(DataResult.ResultSuccess(new
        {
            TotalPermissions = permissions.Count(),
            ResourceCount = grouped.Count,
            Permissions = grouped
        }, "Get success!"));
    }

    /// <summary>
    /// Get permissions for a specific role (admin only)
    /// </summary>
    [HasPermission("roles", "read")]
    [HttpGet("role/{roleName}")]
    public async Task<IActionResult> GetRolePermissions(string roleName)
    {
        var role = await _uow.Roles.Query()
            .Where(r => r.Name == roleName)
            .Include(r => r.Permissions)
            .FirstOrDefaultAsync();

        if (role == null)
            return NotFound(DataResult.ResultError(404, $"Role '{roleName}' not found"));

        var permissions = role.Permissions.Select(p => new
        {
            p.Resource,
            p.Action,
            Permission = $"{p.Resource}:{p.Action}"
        }).OrderBy(p => p.Permission).ToList();

        return Ok(DataResult.ResultSuccess(new
        {
            Role = role.Name,
            PermissionCount = permissions.Count,
            Permissions = permissions
        }, "Get success!"));
    }

    /// <summary>
    /// Assign permission to role (admin only)
    /// </summary>
    [HasPermission("roles", "update")]
    [HttpPost("assign")]
    public async Task<IActionResult> AssignPermissionToRole([FromBody] AssignPermissionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RoleName) ||
            string.IsNullOrWhiteSpace(request.Resource) ||
            string.IsNullOrWhiteSpace(request.Action))
            return BadRequest(DataResult.ResultError(400, "RoleName, Resource, and Action are required"));

        var role = await _uow.Roles.Query()
            .Where(r => r.Name == request.RoleName)
            .Include(r => r.Permissions)
            .FirstOrDefaultAsync();

        if (role == null)
            return NotFound(DataResult.ResultError(404, $"Role '{request.RoleName}' not found"));

        var permission = await _uow.Permissions.Query()
            .FirstOrDefaultAsync(p => p.Resource == request.Resource && p.Action == request.Action);

        if (permission == null)
            return NotFound(DataResult.ResultError(404, $"Permission '{request.Resource}:{request.Action}' not found"));

        if (role.Permissions.Any(p => p.Id == permission.Id))
            return BadRequest(DataResult.ResultError(400, "Role already has this permission"));

        role.Permissions.Add(permission);

        _logger.LogInformation($"Assigned {request.Resource}:{request.Action} to role {request.RoleName}");

        return Ok(DataResult.ResultSuccess(new { message = "Permission assigned successfully" }, "Update success!"));
    }

    /// <summary>
    /// Revoke permission from role (admin only)
    /// </summary>
    [HasPermission("roles", "update")]
    [HttpDelete("revoke")]
    public async Task<IActionResult> RevokePermissionFromRole([FromBody] AssignPermissionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.RoleName) ||
            string.IsNullOrWhiteSpace(request.Resource) ||
            string.IsNullOrWhiteSpace(request.Action))
            return BadRequest(DataResult.ResultError(400, "RoleName, Resource, and Action are required"));

        var role = await _uow.Roles.Query()
            .Where(r => r.Name == request.RoleName)
            .Include(r => r.Permissions)
            .FirstOrDefaultAsync();

        if (role == null)
            return NotFound(DataResult.ResultError(404, $"Role '{request.RoleName}' not found"));

        var permission = role.Permissions.FirstOrDefault(p =>
            p.Resource == request.Resource && p.Action == request.Action);

        if (permission == null)
            return NotFound(DataResult.ResultError(404, $"Role doesn't have permission '{request.Resource}:{request.Action}'"));

        role.Permissions.Remove(permission);

        _logger.LogInformation($"Revoked {request.Resource}:{request.Action} from role {request.RoleName}");

        return Ok(DataResult.ResultSuccess(new { message = "Permission revoked successfully" }, "Update success!"));
    }
}

public class AssignPermissionRequest
{
    public string RoleName { get; set; } = null!;
    public string Resource { get; set; } = null!;
    public string Action { get; set; } = null!;
}
