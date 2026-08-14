namespace NKF.Persistence.Postgres.Base.Services;

public interface ITenantMigrationService
{
    Task MigrateAsync(TenantConnection tenant);
    Task MigrateAllAsync(string url);
}