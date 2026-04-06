namespace SchedulingMS.Application.DTOs.Reservation;

public record CreateReservationWithOrderItemRequest(
    Guid ServiceId,
    string ServiceName,
    decimal UnitPrice,
    int Quantity,
    int DurationMinutes);

public record CreateReservationWithOrderRequest(
    Guid ClientId,
    Guid ProviderEntityId,
    DateTime StartAtUtc,
    IReadOnlyCollection<CreateReservationWithOrderItemRequest> Items);

public record ReservationOrderResponse(
    Guid OrderId,
    Guid ReservationId,
    Guid ClientId,
    Guid ProviderEntityId,
    Guid TechnicianId,
    DateTime StartAtUtc,
    DateTime EndAtUtc,
    string ReservationStatus,
    string OrderStatus,
    decimal TotalAmount);
