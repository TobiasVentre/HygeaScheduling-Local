using SchedulingMS.Application.DTOs.Reservation;
using SchedulingMS.Application.Exceptions;
using SchedulingMS.Application.Interfaces.Queries;
using SchedulingMS.Application.Interfaces.UseCases.Reservation;

namespace SchedulingMS.Application.UseCases.Reservation;

public class GetReservationsByTechnicianUseCase(IReservationQuery reservationQuery) : IGetReservationsByTechnicianUseCase
{
    public async Task<IReadOnlyCollection<ReservationResponse>> ExecuteAsync(Guid technicianId, CancellationToken cancellationToken = default)
    {
        if (technicianId == Guid.Empty) throw new ValidationException("TechnicianId is required.");

        var reservations = await reservationQuery.GetByTechnicianIdAsync(technicianId, cancellationToken);
        return reservations.Select(reservation => new ReservationResponse(
            reservation.Id,
            reservation.ClientId,
            reservation.ProviderEntityId,
            reservation.ServiceId,
            technicianId,
            reservation.StartAtUtc,
            reservation.EndAtUtc,
            reservation.Status,
            reservation.CreatedAtUtc)).ToArray();
    }
}


