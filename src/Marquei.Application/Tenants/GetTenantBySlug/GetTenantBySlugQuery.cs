using Marquei.Application.Abstractions.Messaging;

namespace Marquei.Application.Tenants.GetTenantBySlug;

public record GetTenantBySlugQuery(string Slug) : IQuery<TenantPublicDto?>;