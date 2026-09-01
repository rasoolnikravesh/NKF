using NKF.Domain.Events;

namespace NKF.Domain.SeedWorks;

public interface IAggregateRoot : IEntity
{
    void ClearDomainEvents();

    IReadOnlyList<IDomainEvent> DomainEvents { get; }
}
