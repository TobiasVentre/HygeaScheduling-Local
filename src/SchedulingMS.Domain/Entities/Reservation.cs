using SchedulingMS.Domain.Utilities;
using SchedulingMS.Domain.Enums;

namespace SchedulingMS.Domain.Entities;

public class Reservation
{
    private Reservation() { }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid ClientId { get; private set; }
    public Guid ProviderEntityId { get; private set; }
    public Guid ServiceId { get; private set; }
    public DateTime StartAtUtc { get; private set; }
    public DateTime EndAtUtc { get; private set; }
    public ReservationStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

    public Reservation(Guid clientId, Guid providerEntityId, Guid serviceId, DateTime startAtUtc, DateTime endAtUtc)
    {
        ClientId = clientId;
        ProviderEntityId = providerEntityId;
        ServiceId = serviceId;
        StartAtUtc = ArgentinaDateTime.NormalizeToUtc(startAtUtc);
        EndAtUtc = ArgentinaDateTime.NormalizeToUtc(endAtUtc);
        Status = ReservationStatus.Created;
    }

    public void ChangeStatus(ReservationStatus newStatus)
    {
        if (newStatus == Status) return;
        Status = newStatus;
    }
}


