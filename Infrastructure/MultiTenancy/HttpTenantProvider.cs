using Microsoft.AspNetCore.Http;
using NewRentalCarManagerAPI.Common;
using System.Net;
using System.Security.Claims;

namespace NewRentalCarManagerAPI.Infrastructure.MultiTenancy;

public class HttpTenantProvider : ITenantProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpTenantProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int GetTenantIdOrThrow()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context is null)
        {
            throw new UserFriendlyException((int)HttpStatusCode.BadRequest, "Tenant context is unavailable.");
        }

        var claimValue = context.User.FindFirst("tenantId")?.Value
            ?? context.User.FindFirst("TenantId")?.Value
            ?? context.User.FindFirst("tenant_id")?.Value
            ?? context.User.FindFirst(ClaimTypes.GroupSid)?.Value;

        if (int.TryParse(claimValue, out var tenantFromClaim))
        {
            return tenantFromClaim;
        }

        var headerValue = context.Request.Headers["X-Tenant-Id"].FirstOrDefault();
        if (int.TryParse(headerValue, out var tenantFromHeader))
        {
            return tenantFromHeader;
        }

        throw new UserFriendlyException((int)HttpStatusCode.BadRequest, "TenantId is required. Provide claim 'tenantId' or header 'X-Tenant-Id'.");
    }

    public int? TryGetTenantId()
    {
        var context = _httpContextAccessor.HttpContext;
        if (context is null) return null;

        var claimValue = context.User.FindFirst("tenantId")?.Value
            ?? context.User.FindFirst("TenantId")?.Value
            ?? context.User.FindFirst("tenant_id")?.Value
            ?? context.User.FindFirst(System.Security.Claims.ClaimTypes.GroupSid)?.Value;

        if (int.TryParse(claimValue, out var tenantFromClaim))
            return tenantFromClaim;

        var headerValue = context.Request.Headers["X-Tenant-Id"].FirstOrDefault();
        if (int.TryParse(headerValue, out var tenantFromHeader))
            return tenantFromHeader;

        return null;
    }
}