using Marquei.Domain.Common;

namespace Marquei.Domain.Appointments;

public record AppointmentConfirmed(Guid AppointmentId, Guid TenantId) : IDomainEvent;

public record AppointmentCanceled(Guid AppointmentId, Guid TenantId) : IDomainEvent;
public record AppointmentExpired(Guid AppointmentId, Guid TenantId) : IDomainEvent;
public record AppointmentCompleted(Guid AppointmentId, Guid TenantId) : IDomainEvent;