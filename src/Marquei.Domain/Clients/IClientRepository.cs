using Marquei.Domain.ValueObjects;

namespace Marquei.Domain.Clients;

public interface IClientRepository
{
    Task<Client?> GetByIdAsync(Guid id);

    Task<Client?> GetByPhoneAsync(Guid tenantId, PhoneNumber phone);

    Task AddAsync(Client client);
}