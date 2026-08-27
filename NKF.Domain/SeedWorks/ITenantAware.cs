using System.ComponentModel.DataAnnotations.Schema;

namespace NKF.Domain.SeedWorks;

public interface ITenantAware
{
    Guid TenantId { get; }

    void AssignTenantId(Guid tenantId);
}
