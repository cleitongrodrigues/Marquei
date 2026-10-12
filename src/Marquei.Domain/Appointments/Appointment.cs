using Marquei.Domain.Common;
using Marquei.Domain.Services;
using Marquei.Domain.ValueObjects;

namespace Marquei.Domain.Appointments;

public class Appointment : Entity
{
    private readonly List<AppointmentItem> _items = new();

    public Guid TenantId { get; private set; }
    public Guid ClientId { get; private set; }
    public DateTime StartUtc { get; private set; }
    public DateTime EndUtc { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public DateTime? PaymentExpiresAtUtc { get; private set; }
    public Money DepositAmount { get; private set; } = Money.Zero;

    public IReadOnlyCollection<AppointmentItem> Items => _items.AsReadOnly();

    public Money TotalPrice =>
        _items.Aggregate(Money.Zero, (total, item) => total.Add(item.Price));
    public Money RemainingAmount => TotalPrice.Subtract(DepositAmount);

    private Appointment()
    {
    }

    public static Appointment Schedule(
        Guid tenantId,
        Guid clientId,
        DateTime startUtc,
        IReadOnlyCollection<Service> services,
        decimal depositPercent,
        TimeSpan paymentWindow,
        DateTime nowUtc)
    {
        if (startUtc <= nowUtc)
            throw new DomainException("O horário precisa estar no futuro.");

        if (services.Count == 0)
            throw new DomainException("Escolha pelo menos um serviço.");

        if (services.Any(s => s.TenantId != tenantId))
            throw new DomainException("Serviço não pertence a este salão.");

        if (services.Any(s => !s.IsActive))
            throw new DomainException("Um dos serviços não está disponível para agendamento.");

        if (depositPercent is <= 0 or > 100)
            throw new DomainException("O percentual do sinal deve ser maior que 0 e no máximo 100.");

        var appointment = new Appointment
        {
            TenantId = tenantId,
            ClientId = clientId,
            StartUtc = startUtc,
            Status = AppointmentStatus.PendingPayment,
            PaymentExpiresAtUtc = nowUtc + paymentWindow
        };

        foreach (var service in services)
            appointment._items.Add(AppointmentItem.FromService(service));

        var totalDuration = TimeSpan.FromTicks(
            appointment._items.Sum(i => i.Duration.Ticks));

        appointment.EndUtc = startUtc + totalDuration;
        appointment.DepositAmount = appointment.TotalPrice.Percentage(depositPercent);

        return appointment;
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