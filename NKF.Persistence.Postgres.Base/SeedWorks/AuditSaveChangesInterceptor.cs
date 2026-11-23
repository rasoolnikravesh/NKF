using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NKF.Domain.SharedKernels.ValueObjects;

namespace NKF.Persistence.Postgres.Base.SeedWorks;

public sealed class AuditSaveChangesInterceptor(IHttpContextAccessor httpContextAccessor) : SaveChangesInterceptor
{
    private const string CreatedDateTimeProperty = "CreatedDateTime";
    private const string CreatedByUserIdProperty = "CreatedByUserId";
    private const string UpdatedDateTimeProperty = "UpdatedDateTime";
    private const string UpdatedByUserIdProperty = "UpdatedByUserId";
    private const string IsDeletedProperty = "IsDeleted";
    private const string DeletedByUserIdProperty = "DeletedByUserId";

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        StampAuditFields(eventData.Context);
        return result;
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        StampAuditFields(eventData.Context);
        return ValueTask.FromResult(result);
    }

    private void StampAuditFields(DbContext? context)
    {
        if (context is null)
        {
            return;
        }

        Guid? userId = GetCurrentUserId();
        DateTime now = DateTime.UtcNow;
        foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry in context.ChangeTracker.Entries())
        {
            if (entry.Metadata.FindProperty(CreatedDateTimeProperty) is null)
            {
                continue;
            }

            if (entry.State == EntityState.Added)
            {
                entry.Property(CreatedDateTimeProperty).CurrentValue = CreateDate.Create(now).Value;
                if (userId.HasValue
                    && entry.Property(CreatedByUserIdProperty).CurrentValue is null)
                {
                    entry.Property(CreatedByUserIdProperty).CurrentValue = userId;
                }

                continue;
            }

            if (entry.State != EntityState.Modified)
            {
                continue;
            }

            entry.Property(UpdatedDateTimeProperty).CurrentValue = UpdateDate.Create(now).Value;
            if (userId.HasValue)
            {
                entry.Property(UpdatedByUserIdProperty).CurrentValue = userId;
                if (entry.Property(IsDeletedProperty).CurrentValue is true
                    && entry.Property(DeletedByUserIdProperty).CurrentValue is null)
                {
                    entry.Property(DeletedByUserIdProperty).CurrentValue = userId;
                }
            }
        }
    }

    private Guid? GetCurrentUserId()
    {
        ClaimsPrincipal? user = httpContextAccessor.HttpContext?.User;
        string? value = user?.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? user?.FindFirstValue("sub");
        return Guid.TryParse(value, out Guid userId) && userId != Guid.Empty ? userId : null;
    }
}
