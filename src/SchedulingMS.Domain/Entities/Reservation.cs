using SchedulingMS.Domain.Enums;

namespace SchedulingMS.Domain.Entities;

public class Reservation
{
    private Reservation() { }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public int ClientId { get; private set; }
    public int TechnicianId { get; private set; }
    public Guid ServiceId { get; private set; }
    public DateTime StartAtUtc { get; private set; }
    public DateTime EndAtUtc { get; private set; }
    public ReservationStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;

    public Reservation(int clientId, int technicianId, Guid serviceId, DateTime startAtUtc, DateTime endAtUtc)
    {
        if (clientId <= 0) throw new ArgumentException("ClientId must be greater than 0.");
        if (technicianId <= 0) throw new ArgumentException("TechnicianId must be greater than 0.");
        if (serviceId == Guid.Empty) throw new ArgumentException("ServiceId is required.");
        if (startAtUtc >= endAtUtc) throw new ArgumentException("StartAtUtc must be before EndAtUtc.");

        ClientId = clientId;
        TechnicianId = technicianId;
        ServiceId = serviceId;
        StartAtUtc = DateTime.SpecifyKind(startAtUtc, DateTimeKind.Utc);
        EndAtUtc = DateTime.SpecifyKind(endAtUtc, DateTimeKind.Utc);
        Status = ReservationStatus.Confirmed;
    }

    public void UpdateStatus(ReservationStatus status)
    {
        if (status == Status) return;
        Status = status;
    }

    public bool Overlaps(DateTime otherStartUtc, DateTime otherEndUtc)
        => StartAtUtc < otherEndUtc && otherStartUtc < EndAtUtc;
}
