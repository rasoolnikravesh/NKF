using Microsoft.EntityFrameworkCore;
using NKF.Persistence.Base;
using NKF.Persistence.Postgres.Base.Services;
using Wolverine;
using Wolverine.EntityFrameworkCore;

namespace NKF.Persistence.Postgres.Base.SeedWorks;

public sealed class ScopedTenantDbContextSession<TContext>(
    TContext context,
    IDbContextOutbox outbox,
    TenantContext tenantContext,
    ITenantService tenantService,
    IMessageBus messageBus) : ITenantDbContextSession<TContext>
    where TContext : DbContext
{
    private bool _enrolled;

    public async ValueTask<TContext> GetCurrentAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _ = await ResolveTenantIdAsync();
        EnrollContext();
        return context;
    }

    public async ValueTask<TContext> GetForTenantAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (tenantId == Guid.Empty)
        {
            throw new InvalidOperationException("A tenant id is required to access a tenant database context.");
        }

        Guid currentTenantId = await ResolveTenantIdAsync();
        if (currentTenantId != tenantId)
        {
            throw new InvalidOperationException(
                $"Requested tenant '{tenantId}' does not match the current tenant '{currentTenantId}'.");
        }

        EnrollContext();
        return context;
    }

    public async Task SaveChangesAndFlushMessagesAsync(CancellationToken cancellationToken = default)
    {
        _ = await GetCurrentAsync(cancellationToken);
        await outbox.SaveChangesAndFlushMessagesAsync(cancellationToken);
    }

    public async Task PublishAsync(object message)
    {
        ArgumentNullException.ThrowIfNull(message);
        Guid tenantId = await ResolveTenantIdAsync();
        EnrollContext();
        string? previousTenantId = messageBus.TenantId;
        messageBus.TenantId = tenantId.ToString();
        try
        {
            await messageBus.PublishAsync(message);
        }
        finally
        {
            messageBus.TenantId = previousTenantId;
        }
    }

    private async Task<Guid> ResolveTenantIdAsync()
    {
        if (tenantContext.TenantId != Guid.Empty)
        {
            if (Guid.TryParse(messageBus.TenantId, out Guid messageTenantId)
                && messageTenantId != tenantContext.TenantId)
            {
                throw new InvalidOperationException(
                    $"Current tenant '{tenantContext.TenantId}' does not match the message tenant '{messageTenantId}'.");
            }

            return tenantContext.TenantId;
        }

        if (Guid.TryParse(messageBus.TenantId, out Guid parsedMessageTenantId))
        {
            tenantContext.TenantId = parsedMessageTenantId;
            return parsedMessageTenantId;
        }

        if (!string.IsNullOrWhiteSpace(messageBus.TenantId)
            || !string.IsNullOrWhiteSpace(tenantContext.Domain))
        {
            await tenantService.GetConnectionString();
            if (tenantContext.TenantId != Guid.Empty)
            {
                return tenantContext.TenantId;
            }
        }

        throw new InvalidOperationException(
            $"No tenant id is available for the current {typeof(TContext).Name} persistence operation.");
    }

    private void EnrollContext()
    {
        if (_enrolled)
        {
            return;
        }

        outbox.Enroll(context);
        _enrolled = true;
    }
}
