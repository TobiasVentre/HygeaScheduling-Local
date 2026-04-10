using SchedulingMS.Domain.Enums;

namespace SchedulingMS.Application.DTOs.Reservation;

public record CreateReservationRequest(Guid ClientId, Guid ProviderEntityId, Guid ServiceId, DateTime StartAtUtc, DateTime EndAtUtc);
public record UpdateReservationStatusRequest(ReservationStatus Status, Guid? ChangedByUserId, string? Note);
public record ReassignReservationRequest(Guid TechnicianId, Guid? RequestedByUserId, string? Reason, bool OverrideByAdmin);

public record ReservationResponse(
    Guid Id,
    Guid ClientId,
    Guid ProviderEntityId,
    Guid ServiceId,
    Guid TechnicianId,
    DateTime StartAtUtc,
    DateTime EndAtUtc,
    ReservationStatus Status,
    DateTime CreatedAtUtc);

public record ReservationBusyPeriodResponse(
    Guid TechnicianId,
    DateTime StartAtUtc,
    DateTime EndAtUtc,
    ReservationStatus Status);


