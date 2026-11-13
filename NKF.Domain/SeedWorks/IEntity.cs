namespace NKF.Domain.SeedWorks;

public interface IEntity
{
    Guid Id { get; }
}

public interface IEntity<out TId> : IEntity where TId : EntityId
{
    new TId Id { get; }

    Guid IEntity.Id => Id.Value;
}
