using SchedulingMS.Application.DTOs.Reservation;

namespace SchedulingMS.Application.Interfaces.Ports;

public record CreateExternalServiceOrderItemRequest(Guid ServiceId, string ServiceName, decimal UnitPrice, int Quantity);

public record CreateExternalServiceOrderRequest(
    Guid ReservationId,
    Guid ClientId,
    Guid ProviderEntityId,
    Guid TechnicianId,
    DateTime ScheduledStartAtUtc,
    DateTime ScheduledEndAtUtc,
    IReadOnlyCollection<CreateExternalServiceOrderItemRequest> Items);

public record ExternalServiceOrderResponse(Guid OrderId, string Status, decimal TotalAmount);

public interface IOrderCreationGateway
{
    Task<ExternalServiceOrderResponse> CreateAsync(CreateExternalServiceOrderRequest request, CancellationToken cancellationToken = default);
}
