using Marquei.Application.Abstractions.Messaging;
using Marquei.Domain.Common;
using Marquei.Domain.Tenants;

namespace Marquei.Application.Tenants.CreateTenant;

public class CreateTenantCommandHandler
    : ICommandHandler<CreateTenantCommand, Guid>
{
    private readonly ITenantRepository _tenants;
    private readonly IUnitOfWork _unitOfWork;

    public CreateTenantCommandHandler(
        ITenantRepository tenants,
        IUnitOfWork unitOfWork)
    {
        _tenants = tenants;
        _unitOfWork = unitOfWork;
    }

    public async Task<Guid> HandleAsync(CreateTenantCommand command)
    {
        var tenant = Tenant.Create(command.Name, command.Slug);

        if (await _tenants.SlugExistsAsync(tenant.Slug))
            throw new DomainException("Este link não está disponível.");

        await _tenants.AddAsync(tenant);
        await _unitOfWork.SaveChangesAsync();

        return tenant.Id;
    }
}