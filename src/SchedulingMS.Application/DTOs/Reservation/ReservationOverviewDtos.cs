using SchedulingMS.Domain.Enums;

namespace SchedulingMS.Application.DTOs.Reservation;

public record ReservationOverviewFilter(
    Guid? ClientId,
    Guid? TechnicianId,
    Guid? ProviderEntityId,
    ReservationStatus? Status,
    DateTime? FromUtc,
    DateTime? ToUtc);

public record ReservationStatusCountResponse(string Status, int Count);

public record ReservationOverviewResponse(
    IReadOnlyCollection<ReservationResponse> Items,
    IReadOnlyCollection<ReservationStatusCountResponse> StatusCounts,
    int TotalCount);
