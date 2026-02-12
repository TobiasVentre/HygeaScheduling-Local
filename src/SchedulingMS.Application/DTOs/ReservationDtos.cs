using SchedulingMS.Domain.Enums;

namespace SchedulingMS.Application.DTOs;

public record CreateReservationRequest(int ClientId, int TechnicianId, Guid ServiceId, DateTime StartAtUtc, DateTime EndAtUtc);
public record UpdateReservationStatusRequest(ReservationStatus Status);
public record ReservationResponse(
    Guid Id,
    int ClientId,
    int TechnicianId,
    Guid ServiceId,
    DateTime StartAtUtc,
    DateTime EndAtUtc,
    ReservationStatus Status,
    DateTime CreatedAtUtc);
