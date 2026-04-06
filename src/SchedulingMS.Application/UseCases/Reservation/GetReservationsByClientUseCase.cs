using SchedulingMS.Application.DTOs.Reservation;
using SchedulingMS.Application.Exceptions;
using SchedulingMS.Application.Interfaces.Queries;
using SchedulingMS.Application.Interfaces.UseCases.Reservation;

namespace SchedulingMS.Application.UseCases.Reservation;

public class GetReservationsByClientUseCase(IReservationQuery reservationQuery, IAssignmentQuery assignmentQuery) : IGetReservationsByClientUseCase
{
    public async Task<IReadOnlyCollection<ReservationResponse>> ExecuteAsync(Guid clientId, CancellationToken cancellationToken = default)
    {
        if (clientId == Guid.Empty) throw new ValidationException("ClientId is required.");

        var reservations = await reservationQuery.GetByClientIdAsync(clientId, cancellationToken);
        var result = new List<ReservationResponse>();

        foreach (var reservation in reservations)
        {
            var assignment = await assignmentQuery.GetCurrentByReservationIdAsync(reservation.Id, cancellationToken);
            if (assignment is null) continue;

            result.Add(new ReservationResponse(
                reservation.Id,
                reservation.ClientId,
                reservation.ProviderEntityId,
                reservation.ServiceId,
                assignment.TechnicianId,
                reservation.StartAtUtc,
                reservation.EndAtUtc,
                reservation.Status,
                reservation.CreatedAtUtc));
        }

        return result;
    }
}


