using SchedulingMS.Domain.Enums;

namespace SchedulingMS.Application.Events;

public record ReservationCreatedEvent(
    Guid ReservationId,
    int ClientId,
    int TechnicianId,
    Guid ServiceId,
    DateTime ScheduledAt,
    ReservationStatus Status,
    DateTime OccurredAt);

public record ReservationFinalizedEvent(
    Guid ReservationId,
    int ClientId,
    int TechnicianId,
    Guid ServiceId,
    DateTime ScheduledAt,
    ReservationStatus Status,
    DateTime OccurredAt);
