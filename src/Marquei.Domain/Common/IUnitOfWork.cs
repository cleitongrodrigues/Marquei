namespace Marquei.Domain.Common;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync();
}