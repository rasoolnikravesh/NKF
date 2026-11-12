using FluentResults;

namespace NKF.Application.Base;

public interface ICommandHandler<in TCommand> :
    Mediator.ICommandHandler<TCommand, Result>
    where TCommand : Mediator.ICommand<Result>
{

}

public interface ICommandHandler<in TCommand, TReturnValue> :
    Mediator.ICommandHandler<TCommand, Result<TReturnValue>>
    where TCommand : Mediator.ICommand<Result<TReturnValue>>
{
}
