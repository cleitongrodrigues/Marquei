using Marquei.Domain.Common;
using Marquei.Domain.Services;
using Marquei.Domain.ValueObjects;

namespace Marquei.Domain.Appointments;

public class AppointmentItem : Entity
{
    public Guid ServiceId { get; private set; }
    public string ServiceName { get; private set; } = string.Empty;
    public Money Price { get; private set; } = null!;
    public TimeSpan Duration { get; private set; }

    private AppointmentItem()
    {
    }

    internal static AppointmentItem FromService(Service service)
    {
        return new AppointmentItem
        {
            ServiceId = service.Id,
            ServiceName = service.Name,
            Price = service.Price,
            Duration = service.Duration
        };
    }
}