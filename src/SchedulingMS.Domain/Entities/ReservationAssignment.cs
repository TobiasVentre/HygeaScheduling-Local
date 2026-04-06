using SchedulingMS.Domain.Enums;
using SchedulingMS.Domain.Utilities;

namespace SchedulingMS.Domain.Entities;

public class ReservationAssignment
{
    private ReservationAssignment() { }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid ReservationId { get; private set; }
    public Guid TechnicianId { get; private set; }
    public AssignmentType AssignmentType { get; private set; }
    public DateTime AssignedAtUtc { get; private set; }
    public Guid? AssignedByUserId { get; private set; }
    public string? Reason { get; private set; }
    public bool IsCurrent { get; private set; }

    public ReservationAssignment(Guid reservationId, Guid technicianId, AssignmentType assignmentType, DateTime assignedAtUtc, Guid? assignedByUserId, string? reason, bool isCurrent)
    {
        ReservationId = reservationId;
        TechnicianId = technicianId;
        AssignmentType = assignmentType;
        AssignedAtUtc = ArgentinaDateTime.NormalizeToUtc(assignedAtUtc);
        AssignedByUserId = assignedByUserId;
        Reason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
        IsCurrent = isCurrent;
    }

    public void MarkAsHistorical()
    {
        IsCurrent = false;
    }
}


