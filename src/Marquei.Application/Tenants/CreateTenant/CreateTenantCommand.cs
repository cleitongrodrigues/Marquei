using Marquei.Application.Abstractions.Messaging;

namespace Marquei.Application.Tenants.CreateTenant;

public record CreateTenantCommand(string Name, string Slug) : ICommand<Guid>;