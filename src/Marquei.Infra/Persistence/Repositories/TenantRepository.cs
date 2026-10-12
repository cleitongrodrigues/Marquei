using Marquei.Domain.Tenants;
using Microsoft.EntityFrameworkCore;

namespace Marquei.Infra.Persistence.Repositories;

public class TenantRepository : ITenantRepository
{
    private readonly AppDbContext _context;

    public TenantRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<Tenant?> GetByIdAsync(Guid id)
    {
        return _context.Tenants.FirstOrDefaultAsync(t => t.Id == id);
    }

    public Task<Tenant?> GetBySlugAsync(string slug)
    {
        return _context.Tenants.FirstOrDefaultAsync(t => t.Slug == slug);
    }

    public Task<bool> SlugExistsAsync(string slug)
    {
        return _context.Tenants.AnyAsync(t => t.Slug == slug);
    }

    public async Task AddAsync(Tenant tenant)
    {
        await _context.Tenants.AddAsync(tenant);
    }
}