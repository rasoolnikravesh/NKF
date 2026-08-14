using FluentResults;
using MediatR;

namespace NKF.Application.Base;

public interface ICommandHandler<in TCommand> :
	IRequestHandler<TCommand, Result>
	where TCommand : IRequest<Result>
{

}

public interface ICommandHandler<in TCommand, TReturnValue> :
    IRequestHandler<TCommand, Result<TReturnValue>>
	where TCommand : IRequest<Result<TReturnValue>>
{
}