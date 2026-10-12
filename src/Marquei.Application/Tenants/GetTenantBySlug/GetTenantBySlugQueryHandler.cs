using Marquei.Application.Abstractions.Messaging;
using Marquei.Domain.Tenants;

namespace Marquei.Application.Tenants.GetTenantBySlug;

public class GetTenantBySlugQueryHandler
    : IQueryHandler<GetTenantBySlugQuery, TenantPublicDto?>
{
    private readonly ITenantRepository _tenants;

    public GetTenantBySlugQueryHandler(ITenantRepository tenants)
    {
        _tenants = tenants;
    }

    public async Task<TenantPublicDto?> HandleAsync(GetTenantBySlugQuery query)
    {
        var slug = query.Slug.Trim().ToLowerInvariant();

        var tenant = await _tenants.GetBySlugAsync(slug);

        if (tenant is null || !tenant.IsActive)
            return null;

        return new TenantPublicDto(tenant.Name, tenant.Slug, tenant.DepositPercent);
    }
}