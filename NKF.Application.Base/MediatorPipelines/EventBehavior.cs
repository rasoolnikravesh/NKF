using MediatR;

namespace NKF.Application.Base.MediatorPipelines;

public class EventBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    // public IQueryUnitOfWork UnitOfWork { get; }
    //
    // public EventBehavior(IQueryUnitOfWork unitOfWork)
    // {
    // 	UnitOfWork = unitOfWork;
    // }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {

        //var objType = request
        //	.GetType()
        //	.GetInterfaces()
        //	.SelectMany(x => x.GenericTypeArguments)
        //	.FirstOrDefault(x => x.GetInterfaces().Any(y => y == typeof(IAggregateRoot)));

        //ConstructorInfo? instanceCon =
        //	objType.GetConstructor(Type.EmptyTypes);
        //if (instanceCon != null)
        //{

        //	object instance = instanceCon.Invoke(new object?[] { });
        //	dynamic t = instance;

        //	var r = UnitOfWork.GetQueryRepository(Convert.ChangeType(t, objType));


        //}

        TResponse response = await next();


        return response;
    }
}
