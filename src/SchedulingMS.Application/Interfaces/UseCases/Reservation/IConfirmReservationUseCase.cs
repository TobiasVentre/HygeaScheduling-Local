using SchedulingMS.Application.DTOs.Reservation;

namespace SchedulingMS.Application.Interfaces.UseCases.Reservation;

public interface IConfirmReservationUseCase
{
    Task<ReservationResponse> ExecuteAsync(Guid reservationId, Guid? reviewedByUserId, string? note, CancellationToken cancellationToken = default);
}
