using Microsoft.EntityFrameworkCore;
using NKF.Persistence.Base;
using NKF.Persistence.Postgres.Base.Services;
using Wolverine;
using Wolverine.EntityFrameworkCore;

namespace NKF.Persistence.Postgres.Base.SeedWorks;

public sealed class TenantDbContextSession<TContext>(
    IDbContextOutboxFactory outboxFactory,
    TenantContext tenantContext,
    ITenantService tenantService,
    IMessageBus messageBus) : ITenantDbContextSession<TContext>
    where TContext : DbContext
{
    private readonly SemaphoreSlim _creationLock = new(1, 1);
    private IDbContextOutbox<TContext>? _outbox;
    private Guid? _tenantId;
    private bool _disposed;

    public async ValueTask<TContext> GetCurrentAsync(
        CancellationToken cancellationToken = default)
    {
        Guid tenantId = await ResolveCurrentTenantIdAsync();
        return await GetForTenantAsync(tenantId, cancellationToken);
    }

    public async ValueTask<TContext> GetForTenantAsync(
        Guid tenantId,
        CancellationToken cancellationToken = default)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        if (tenantId == Guid.Empty)
        {
            throw new InvalidOperationException("A tenant id is required to create a tenant database context.");
        }

        if (tenantContext.TenantId != Guid.Empty && tenantContext.TenantId != tenantId)
        {
            throw new InvalidOperationException(
                $"Requested tenant '{tenantId}' does not match the current tenant '{tenantContext.TenantId}'.");
        }

        if (Guid.TryParse(messageBus.TenantId, out Guid messageTenantId) && messageTenantId != tenantId)
        {
            throw new InvalidOperationException(
                $"Requested tenant '{tenantId}' does not match the message tenant '{messageTenantId}'.");
        }

        tenantContext.TenantId = tenantId;

        await _creationLock.WaitAsync(cancellationToken);
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            if (_tenantId.HasValue && _tenantId.Value != tenantId)
            {
                throw new InvalidOperationException(
                    $"This database session is already bound to tenant '{_tenantId.Value}' and cannot be reused for tenant '{tenantId}'.");
            }

            if (_outbox is null)
            {
                _outbox = await outboxFactory.CreateForTenantAsync<TContext>(
                    tenantId.ToString(),
                    cancellationToken);
                _tenantId = tenantId;
            }

            return _outbox.DbContext;
        }
        finally
        {
            _creationLock.Release();
        }
    }

    public async Task SaveChangesAndFlushMessagesAsync(CancellationToken cancellationToken = default)
    {
        _ = await GetCurrentAsync(cancellationToken);
        await _outbox!.SaveChangesAndFlushMessagesAsync(cancellationToken);
    }

    public async Task PublishAsync(object message)
    {
        ArgumentNullException.ThrowIfNull(message);
        _ = await GetCurrentAsync();
        await _outbox!.PublishAsync(message);
    }

    public async ValueTask DisposeAsync()
    {
        await _creationLock.WaitAsync();
        try
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            if (_outbox is not null)
            {
                await _outbox.DbContext.DisposeAsync();
                _outbox = null;
            }
        }
        finally
        {
            _creationLock.Release();
        }
    }

    private async Task<Guid> ResolveCurrentTenantIdAsync()
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
}
