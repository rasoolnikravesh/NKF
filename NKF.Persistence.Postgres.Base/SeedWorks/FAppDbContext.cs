using Microsoft.EntityFrameworkCore;

namespace NKF.Persistence.Postgres.Base.SeedWorks;

public abstract class FAppDbContext(DbContextOptions options) : DbContext(options)
{
}
