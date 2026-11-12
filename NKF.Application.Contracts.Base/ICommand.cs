using FluentResults;

namespace NKF.Application.Contracts.Base;

public interface ICommand : Mediator.ICommand<Result>
{

}

public interface ICommand<T> : Mediator.ICommand<Result<T>>
{

}
