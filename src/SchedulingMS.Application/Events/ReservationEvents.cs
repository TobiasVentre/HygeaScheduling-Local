using SchedulingMS.Domain.Enums;

namespace SchedulingMS.Application.Events;

public abstract record ReservationEvent(Guid ReservationId, DateTime OccurredAtUtc);

public sealed record ReservationCreatedEvent(
    Guid ReservationId,
    Guid ClientId,
    Guid ProviderEntityId,
    Guid ServiceId,
    Guid TechnicianId,
    DateTime StartAtUtc,
    DateTime EndAtUtc,
    DateTime OccurredAtUtc) : ReservationEvent(ReservationId, OccurredAtUtc);

public sealed record ReservationReassignedEvent(
    Guid ReservationId,
    Guid PreviousTechnicianId,
    Guid NewTechnicianId,
    DateTime OccurredAtUtc) : ReservationEvent(ReservationId, OccurredAtUtc);

public sealed record ReservationStatusChangedEvent(
    Guid ReservationId,
    ReservationStatus PreviousStatus,
    ReservationStatus NewStatus,
    DateTime OccurredAtUtc) : ReservationEvent(ReservationId, OccurredAtUtc);


