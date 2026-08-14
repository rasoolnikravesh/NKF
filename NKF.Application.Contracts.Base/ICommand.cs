using FluentResults;

namespace NKF.Application.Contracts.Base;

public interface ICommand : MediatR.IRequest<Result>
{

}

public interface ICommand<T> : MediatR.IRequest<Result<T>>
{

}