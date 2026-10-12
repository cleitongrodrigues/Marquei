namespace Marquei.Application.Tenants.GetTenantBySlug;

public record TenantPublicDto(
    string Name,
    string Slug,
    decimal DepositPercent);