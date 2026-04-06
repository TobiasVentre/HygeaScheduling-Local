using SchedulingMS.Application.DTOs.Reservation;
using SchedulingMS.Application.Interfaces.Queries;
using SchedulingMS.Application.Interfaces.UseCases.Reservation;

namespace SchedulingMS.Application.UseCases.Reservation;

public class GetReservationsOverviewUseCase(
    IReservationQuery reservationQuery,
    IAssignmentQuery assignmentQuery) : IGetReservationsOverviewUseCase
{
    public async Task<ReservationOverviewResponse> ExecuteAsync(ReservationOverviewFilter filter, CancellationToken cancellationToken = default)
    {
        var reservations = await reservationQuery.SearchAsync(filter, cancellationToken);
        var items = new List<ReservationResponse>(reservations.Count);

        foreach (var reservation in reservations)
        {
            var assignment = await assignmentQuery.GetCurrentByReservationIdAsync(reservation.Id, cancellationToken);
            items.Add(new ReservationResponse(
                reservation.Id,
                reservation.ClientId,
                reservation.ProviderEntityId,
                reservation.ServiceId,
                assignment?.TechnicianId ?? Guid.Empty,
                reservation.StartAtUtc,
                reservation.EndAtUtc,
                reservation.Status,
                reservation.CreatedAtUtc));
        }

        var counts = reservations
            .GroupBy(x => x.Status)
            .OrderBy(x => x.Key)
            .Select(x => new ReservationStatusCountResponse(x.Key.ToString(), x.Count()))
            .ToArray();

        return new ReservationOverviewResponse(items, counts, reservations.Count);
    }
}
