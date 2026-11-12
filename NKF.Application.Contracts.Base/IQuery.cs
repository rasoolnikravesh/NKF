using FluentResults;

namespace NKF.Application.Contracts.Base;

public interface IQuery : Mediator.IQuery<Result>
{

}

public interface IQuery<TResponse> : Mediator.IQuery<Result<TResponse>>
{
}
