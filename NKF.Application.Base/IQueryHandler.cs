using FluentResults;

namespace NKF.Application.Base;

public interface IQueryHandler<in TCommand, TReturnValue> :
    Mediator.IQueryHandler<TCommand, Result<TReturnValue>>
    where TCommand : Mediator.IQuery<Result<TReturnValue>>
{
}
