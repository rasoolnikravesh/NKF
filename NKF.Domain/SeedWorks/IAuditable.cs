using NKF.Domain.SharedKernels.ValueObjects;

namespace NKF.Domain.SeedWorks;

public interface IAuditable
{
    Guid? CreatedByUserId { get; }
    CreateDate CreatedDateTime { get; }

    Guid? UpdatedByUserId { get; }
    UpdateDate UpdatedDateTime { get; }

    Guid? OwnedByUserId { get; }

    bool IsDeleted { get; }
    Guid? DeletedByUserId { get; }
}
