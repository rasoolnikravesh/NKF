using FluentResults;
using MediatR;

namespace NKF.Application.Base;

public interface IQueryHandler<in TCommand, TReturnValue> :
    IRequestHandler<TCommand, Result<TReturnValue>>
    where TCommand : IRequest<Result<TReturnValue>>
{
}