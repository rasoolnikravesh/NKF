using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NKF.Persistence.Base;

namespace NKF.Persistence.Postgres.Base.SeedWorks;

public static class TenantPersistenceServiceCollectionExtensions
{
    public static IServiceCollection AddWolverineTenantPersistence<TContext>(this IServiceCollection services)
        where TContext : DbContext
    {
        services.AddScoped<ITenantDbContextSession<TContext>, TenantDbContextSession<TContext>>();
        services.AddScoped<ISaveChangesOperation, TenantSaveChangesOperation<TContext>>();
        services.AddScoped<ITransactionManager, TenantTransactionManager<TContext>>();
        return services;
    }

    public static IServiceCollection AddScopedWolverineTenantPersistence<TContext>(this IServiceCollection services)
        where TContext : DbContext
    {
        services.AddScoped<ITenantDbContextSession<TContext>, ScopedTenantDbContextSession<TContext>>();
        services.AddScoped<ISaveChangesOperation, TenantSaveChangesOperation<TContext>>();
        services.AddScoped<ITransactionManager, TenantTransactionManager<TContext>>();
        return services;
    }
}
