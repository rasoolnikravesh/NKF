using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace NKF.Persistence.Postgres.Base.SeedWorks;

public static class AuditSaveChangesInterceptorExtensions
{
    public static AuditSaveChangesInterceptor AddAuditSaveChangesInterceptor(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        IHttpContextAccessor httpContextAccessor = new HttpContextAccessor();
        services.Replace(ServiceDescriptor.Singleton<IHttpContextAccessor>(httpContextAccessor));
        AuditSaveChangesInterceptor interceptor = new(httpContextAccessor);
        services.AddSingleton(interceptor);
        return interceptor;
    }

    public static DbContextOptionsBuilder AddAuditSaveChangesInterceptor(
        this DbContextOptionsBuilder optionsBuilder,
        AuditSaveChangesInterceptor interceptor)
    {
        return optionsBuilder.AddInterceptors(interceptor);
    }
}
