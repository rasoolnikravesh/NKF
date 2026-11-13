namespace NKF.Domain.Events;

public interface IIntegrationEvent
{
    DateTime OccurredOn { get; }
}
