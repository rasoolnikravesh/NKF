namespace NKF.Domain.SeedWorks;

public interface IEntity
{
    Guid Id { get; }
}

public interface IEntity<TId> : IEntity where TId : EntityId
{
    new TId Id { get; }
}
