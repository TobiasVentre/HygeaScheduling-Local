using SchedulingMS.Application.DTOs.Reservation;
using SchedulingMS.Application.Exceptions;
using SchedulingMS.Application.Interfaces.Queries;
using SchedulingMS.Application.Interfaces.UseCases.Reservation;

namespace SchedulingMS.Application.UseCases.Reservation;

public class GetReservationByIdUseCase(IReservationQuery reservationQuery, IAssignmentQuery assignmentQuery) : IGetReservationByIdUseCase
{
    public async Task<ReservationResponse?> ExecuteAsync(Guid reservationId, CancellationToken cancellationToken = default)
    {
        var reservation = await reservationQuery.GetByIdAsync(reservationId, cancellationToken);
        if (reservation is null) return null;

        var assignment = await assignmentQuery.GetCurrentByReservationIdAsync(reservation.Id, cancellationToken)
            ?? throw new NotFoundException($"Reservation {reservation.Id} has no current assignment.");

        return new ReservationResponse(
            reservation.Id,
            reservation.ClientId,
            reservation.ProviderEntityId,
            reservation.ServiceId,
            assignment.TechnicianId,
            reservation.StartAtUtc,
            reservation.EndAtUtc,
            reservation.Status,
            reservation.CreatedAtUtc);
    }
}


