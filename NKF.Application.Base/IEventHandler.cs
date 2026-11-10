using MediatR;
using NKF.Domain.Events;

namespace NKF.Application.Base;

public interface IEventHandler<in TEvent> : INotificationHandler<TEvent> where TEvent : IDomainEvent
{

}
