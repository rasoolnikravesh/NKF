using FluentResults;

namespace NKF.Application.Contracts.Base;

public interface IQuery : MediatR.IRequest<Result>
{

}

public interface IQuery<TResponse> : MediatR.IRequest<Result<TResponse>>
{
}
