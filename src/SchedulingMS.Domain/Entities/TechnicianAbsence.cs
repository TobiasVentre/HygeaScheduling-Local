using SchedulingMS.Domain.Utilities;

namespace SchedulingMS.Domain.Entities;

public class TechnicianAbsence
{
    private TechnicianAbsence() { }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid TechnicianId { get; private set; }
    public Guid ProviderEntityId { get; private set; }
    public DateTime StartAtUtc { get; private set; }
    public DateTime EndAtUtc { get; private set; }
    public string Reason { get; private set; } = string.Empty;

    public TechnicianAbsence(Guid technicianId, Guid providerEntityId, DateTime startAtUtc, DateTime endAtUtc, string reason)
    {
        TechnicianId = technicianId;
        ProviderEntityId = providerEntityId;
        StartAtUtc = ArgentinaDateTime.NormalizeToUtc(startAtUtc);
        EndAtUtc = ArgentinaDateTime.NormalizeToUtc(endAtUtc);
        Reason = string.IsNullOrWhiteSpace(reason) ? "Scheduled absence" : reason.Trim();
    }

    public void Update(DateTime startAtUtc, DateTime endAtUtc, string reason)
    {
        StartAtUtc = ArgentinaDateTime.NormalizeToUtc(startAtUtc);
        EndAtUtc = ArgentinaDateTime.NormalizeToUtc(endAtUtc);
        Reason = string.IsNullOrWhiteSpace(reason) ? Reason : reason.Trim();
    }

    public bool Overlaps(DateTime otherStartUtc, DateTime otherEndUtc)
        => StartAtUtc < otherEndUtc && otherStartUtc < EndAtUtc;
}


