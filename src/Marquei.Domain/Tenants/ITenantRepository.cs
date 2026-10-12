namespace Marquei.Domain.Tenants;

public interface ITenantRepository
{
    Task<Tenant?> GetByIdAsync(Guid id);

    Task<Tenant?> GetBySlugAsync(string slug);

    Task<bool> SlugExistsAsync(string slug);

    Task AddAsync(Tenant tenant);
}