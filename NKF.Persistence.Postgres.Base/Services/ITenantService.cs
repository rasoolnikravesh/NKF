namespace NKF.Persistence.Postgres.Base.Services;

public interface ITenantService : IDisposable, IAsyncDisposable
{
    // این متد کانکشن استرینگ اون فروشگاهی که الان ریکوئست داده رو برمی‌گردونه
    Task<string> GetConnectionString();
}
