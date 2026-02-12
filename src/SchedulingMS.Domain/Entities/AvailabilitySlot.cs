namespace SchedulingMS.Domain.Entities;

public class AvailabilitySlot
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public int TechnicianId { get; private set; }
    public DateTime StartAtUtc { get; private set; }
    public DateTime EndAtUtc { get; private set; }

    public AvailabilitySlot(int technicianId, DateTime startAtUtc, DateTime endAtUtc)
    {
        if (technicianId <= 0) throw new ArgumentException("TechnicianId must be greater than 0.");
        if (startAtUtc >= endAtUtc) throw new ArgumentException("StartAtUtc must be before EndAtUtc.");

        TechnicianId = technicianId;
        StartAtUtc = DateTime.SpecifyKind(startAtUtc, DateTimeKind.Utc);
        EndAtUtc = DateTime.SpecifyKind(endAtUtc, DateTimeKind.Utc);
    }

    public void Update(DateTime startAtUtc, DateTime endAtUtc)
    {
        if (startAtUtc >= endAtUtc) throw new ArgumentException("StartAtUtc must be before EndAtUtc.");

        StartAtUtc = DateTime.SpecifyKind(startAtUtc, DateTimeKind.Utc);
        EndAtUtc = DateTime.SpecifyKind(endAtUtc, DateTimeKind.Utc);
    }

    public bool Overlaps(DateTime otherStartUtc, DateTime otherEndUtc)
        => StartAtUtc < otherEndUtc && otherStartUtc < EndAtUtc;
}
