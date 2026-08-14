using MediatR;

namespace NKF.Domain.Events;

public interface IDomainEvent : INotification
{
    DateTime OccurredOn { get; }
}