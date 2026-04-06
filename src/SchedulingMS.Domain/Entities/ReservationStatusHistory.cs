using SchedulingMS.Domain.Enums;
using SchedulingMS.Domain.Utilities;

namespace SchedulingMS.Domain.Entities;

public class ReservationStatusHistory
{
    private ReservationStatusHistory() { }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid ReservationId { get; private set; }
    public ReservationStatus? PreviousStatus { get; private set; }
    public ReservationStatus NewStatus { get; private set; }
    public DateTime ChangedAtUtc { get; private set; }
    public Guid? ChangedByUserId { get; private set; }
    public string? Note { get; private set; }

    public ReservationStatusHistory(Guid reservationId, ReservationStatus? previousStatus, ReservationStatus newStatus, DateTime changedAtUtc, Guid? changedByUserId, string? note)
    {
        ReservationId = reservationId;
        PreviousStatus = previousStatus;
        NewStatus = newStatus;
        ChangedAtUtc = ArgentinaDateTime.NormalizeToUtc(changedAtUtc);
        ChangedByUserId = changedByUserId;
        Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
    }
}


