using SchedulingMS.Application.DTOs.Reservation;

namespace SchedulingMS.Application.Interfaces.UseCases.Reservation;

public interface ICreateReservationWithOrderUseCase
{
    Task<ReservationOrderResponse> ExecuteAsync(CreateReservationWithOrderRequest request, CancellationToken cancellationToken = default);
}
