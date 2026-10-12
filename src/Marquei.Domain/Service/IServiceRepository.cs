namespace Marquei.Domain.Services;

public interface IServiceRepository
{
    Task<Service?> GetByIdAsync(Guid id);

    Task<IReadOnlyList<Service>> GetByIdsAsync(Guid tenantId, IReadOnlyCollection<Guid> ids);

    Task AddAsync(Service service);
}