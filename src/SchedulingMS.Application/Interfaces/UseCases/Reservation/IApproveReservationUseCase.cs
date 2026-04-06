using SchedulingMS.Application.DTOs.Reservation;

namespace SchedulingMS.Application.Interfaces.UseCases.Reservation;

public interface IApproveReservationUseCase
{
    Task<ReservationResponse> ExecuteAsync(Guid reservationId, Guid? reviewedByUserId, string? note, CancellationToken cancellationToken = default);
}
