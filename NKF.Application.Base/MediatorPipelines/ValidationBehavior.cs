// using MediatR;
//
// namespace NKF.Application.Base.MediatorPipelines;
//
// public class ValidationBehaviour<TRequest, TResponse> : object,
//     IPipelineBehavior<TRequest, TResponse>
//     where TRequest : IRequest<TResponse>
// {
//
//     public ValidationBehaviour
//         (IEnumerable<IValidator<TRequest>> validators)
//     {
//         Validators = validators;
//     }
//
//     protected IEnumerable<IValidator<TRequest>> Validators { get; }
//
//     public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
//     {
//         if (Validators.Any())
//         {
//             ValidationContext<TRequest> context =
//                 new ValidationContext<TRequest>(request);
//
//             ValidationResult[] validationResults =
//                 await Task.WhenAll
//                     (Validators.Select(v => v.ValidateAsync(context, cancellationToken)));
//
//             List<ValidationFailure> failures =
//                     validationResults
//                         .SelectMany(current => current.Errors)
//                         .Where(current => current != null)
//                         .ToList()
//                 ;
//
//             if (failures.Count != 0)
//             {
//
//
//                 throw new ValidationException(errors: failures);
//             }
//         }
//
//         return await next();
//     }
// }
