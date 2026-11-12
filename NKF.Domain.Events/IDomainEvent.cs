namespace NKF.Domain.Events;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
