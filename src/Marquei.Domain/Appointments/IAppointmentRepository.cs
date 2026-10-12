namespace Marquei.Domain.Appointments;

public interface IAppointmentRepository
{
    Task<Appointment?> GetByIdAsync(Guid id);

    Task AddAsync(Appointment appointment);

    Task<bool> HasConflictAsync(Guid tenantId, DateTime startUtc, DateTime endUtc);
}