namespace NKF.Persistence.Postgres.Base.Services;

public class TenantConnection
{
    public Guid Id { get; set; }
    public string? ConnectionString { get; set; }
    public string? Name { get; set; }
}
