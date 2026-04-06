using SchedulingMS.Domain.Utilities;

namespace SchedulingMS.Domain.Entities;

public class AvailabilitySlot
{
    private AvailabilitySlot() { }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid TechnicianId { get; private set; }
    public Guid ProviderEntityId { get; private set; }
    public DateTime StartAtUtc { get; private set; }
    public DateTime EndAtUtc { get; private set; }

    public AvailabilitySlot(Guid technicianId, Guid providerEntityId, DateTime startAtUtc, DateTime endAtUtc)
    {
        TechnicianId = technicianId;
        ProviderEntityId = providerEntityId;
        StartAtUtc = ArgentinaDateTime.NormalizeToUtc(startAtUtc);
        EndAtUtc = ArgentinaDateTime.NormalizeToUtc(endAtUtc);
    }

    public void Update(DateTime startAtUtc, DateTime endAtUtc)
    {
        StartAtUtc = ArgentinaDateTime.NormalizeToUtc(startAtUtc);
        EndAtUtc = ArgentinaDateTime.NormalizeToUtc(endAtUtc);
    }

    public bool Covers(DateTime startAtUtc, DateTime endAtUtc)
        => StartAtUtc <= startAtUtc && EndAtUtc >= endAtUtc;

    public bool Overlaps(DateTime otherStartUtc, DateTime otherEndUtc)
        => StartAtUtc < otherEndUtc && otherStartUtc < EndAtUtc;
}


