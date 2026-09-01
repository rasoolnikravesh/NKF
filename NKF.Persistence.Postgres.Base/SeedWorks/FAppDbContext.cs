using Microsoft.EntityFrameworkCore;
using NKF.Persistence.Postgres.Base.Services;

namespace NKF.Persistence.Postgres.Base.SeedWorks;

public abstract class FAppDbContext(DbContextOptions options) : DbContext(options)
{
}
