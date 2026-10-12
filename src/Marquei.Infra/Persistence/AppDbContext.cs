using Marquei.Domain.Common;
using Marquei.Domain.Tenants;
using Microsoft.EntityFrameworkCore;

namespace Marquei.Infra.Persistence;

public class AppDbContext : DbContext, IUnitOfWork
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    Task<int> IUnitOfWork.SaveChangesAsync() => SaveChangesAsync();
}