using MediatR;
using NKF.Domain.Events;
using NKF.Domain.SeedWorks;

namespace NKF.Application.Base;

public interface IEventHandler<in TEvent> : INotificationHandler<TEvent> where TEvent : IDomainEvent
{

}
