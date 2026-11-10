namespace NKF.Domain.SeedWorks;

public interface ITenantAware
{
    Guid TenantId { get; }

    void AssignTenantId(Guid tenantId);
}
