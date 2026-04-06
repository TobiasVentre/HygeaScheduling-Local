using SchedulingMS.Application.DTOs.Reservation;
using SchedulingMS.Application.Interfaces.UseCases.Reservation;
using SchedulingMS.Domain.Enums;

namespace SchedulingMS.Application.UseCases.Reservation;

public class ConfirmReservationUseCase(IUpdateReservationStatusUseCase updateReservationStatusUseCase) : IConfirmReservationUseCase
{
    public Task<ReservationResponse> ExecuteAsync(Guid reservationId, Guid? reviewedByUserId, string? note, CancellationToken cancellationToken = default)
        => updateReservationStatusUseCase.ExecuteAsync(
            reservationId,
            new UpdateReservationStatusRequest(ReservationStatus.Confirmed, reviewedByUserId, note),
            cancellationToken);
}
