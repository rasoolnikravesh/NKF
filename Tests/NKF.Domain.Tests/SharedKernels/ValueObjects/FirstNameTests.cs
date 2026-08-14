using FluentResults;
using NKF.Domain.SharedKernels.ValueObjects;
using NKF.Domain.Tests.Base;

namespace NKF.Domain.Tests.SharedKernels.ValueObjects;

public class FirstNameTests : GenericStringValueObjectTests<FirstName>
{
    public override Func<string?, Result<FirstName>> CreateFunc => FirstName.Create;
    public override int MinLength => FirstName.MinLength;
    public override int MaxLength => FirstName.MaxLength;
}