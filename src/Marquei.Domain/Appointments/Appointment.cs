using Marquei.Domain.Common;

namespace Marquei.Domain.Appointments;

public class Appointment : Entity
{
    public Guid TenantId { get; private set; }
    public Guid ClientId { get; private set; }
    public DateTime StartUtc { get; private set; }
    public DateTime EndUtc { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public DateTime? PaymentExpiresAtUtc { get; private set; }

    private Appointment()
    {
    }

    public static Appointment Schedule(
        Guid tenantId,
        Guid clientId,
        DateTime startUtc,
        TimeSpan duration,
        TimeSpan paymentWindow,
        DateTime nowUtc)
    {
        if (startUtc <= nowUtc)
            throw new DomainException("O horário precisa estar no futuro.");

        if (duration <= TimeSpan.Zero)
            throw new DomainException("A duração precisa ser maior que zero.");

        return new Appointment
        {
            TenantId = tenantId,
            ClientId = clientId,
            StartUtc = startUtc,
            EndUtc = startUtc + duration,
            Status = AppointmentStatus.PendingPayment,
            PaymentExpiresAtUtc = nowUtc + paymentWindow
        };
    }

    public void ConfirmPayment(DateTime nowUtc)
    {
        if (Status != AppointmentStatus.PendingPayment)
            throw new DomainException("Só é possível confirmar agendamentos pendentes.");

        if (nowUtc > PaymentExpiresAtUtc)
            throw new DomainException("A reserva expirou.");

        Status = AppointmentStatus.Confirmed;
        PaymentExpiresAtUtc = null;
        Raise(new AppointmentConfirmed(Id, TenantId));
    }

    public void Cancel()
    {
        if (Status is AppointmentStatus.Completed or AppointmentStatus.Canceled)
            throw new DomainException("Este agendamento não pode ser cancelado.");

        Status = AppointmentStatus.Canceled;
        Raise(new AppointmentCanceled(Id, TenantId));
    }

    public void Expire(DateTime nowUtc)
    {
        if (Status != AppointmentStatus.PendingPayment)
            throw new DomainException("Só é possível expirar agendamentos pendentes.");

        if (nowUtc <= PaymentExpiresAtUtc)
            throw new DomainException("O prazo de pagamento ainda não terminou.");

        Status = AppointmentStatus.Expired;
        PaymentExpiresAtUtc = null;
        Raise(new AppointmentExpired(Id, TenantId));
    }

    public void Complete(DateTime nowUtc)
    {
        if (Status != AppointmentStatus.Confirmed)
            throw new DomainException("Só é possível concluir agendamentos confirmados.");

        if (nowUtc < StartUtc)
            throw new DomainException("O atendimento ainda não começou.");

        Status = AppointmentStatus.Completed;
        Raise(new AppointmentCompleted(Id, TenantId));
    }
}