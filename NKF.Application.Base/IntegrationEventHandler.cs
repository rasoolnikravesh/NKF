using NKF.Domain.Events;

namespace NKF.Application.Base;

public abstract class IntegrationEventHandler<TEvent>
    where TEvent : IIntegrationEvent
{
    public abstract Task HandleAsync(TEvent message, CancellationToken cancellationToken);
}
