namespace NewRentalCarManagerAPI.Infrastructure.MultiTenancy;

public interface ITenantProvider
{
    int GetTenantIdOrThrow();
    int? TryGetTenantId();
}