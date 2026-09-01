using FluentResults;
using NKF.Domain.SharedKernels.ValueObjects;
using NKF.Domain.Tests.Base;

namespace NKF.Domain.Tests.SharedKernels.ValueObjects;

public class LastNameTests : GenericStringValueObjectTests<LastName>
{
    public override Func<string?, Result<LastName>> CreateFunc => LastName.Create;
    public override int MinLength => LastName.MinLength;
    public override int MaxLength => LastName.MaxLength;
}
