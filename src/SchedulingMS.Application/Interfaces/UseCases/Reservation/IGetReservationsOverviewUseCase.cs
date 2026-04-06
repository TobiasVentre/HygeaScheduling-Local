using SchedulingMS.Application.DTOs.Reservation;

namespace SchedulingMS.Application.Interfaces.UseCases.Reservation;

public interface IGetReservationsOverviewUseCase
{
    Task<ReservationOverviewResponse> ExecuteAsync(ReservationOverviewFilter filter, CancellationToken cancellationToken = default);
}
