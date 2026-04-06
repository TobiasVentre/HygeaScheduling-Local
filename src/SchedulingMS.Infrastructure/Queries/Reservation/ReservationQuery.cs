using Microsoft.EntityFrameworkCore;
using SchedulingMS.Application.DTOs.Reservation;
using SchedulingMS.Application.Interfaces.Queries;
using SchedulingMS.Domain.Entities;
using SchedulingMS.Domain.Enums;
using SchedulingMS.Infrastructure.Persistence;

namespace SchedulingMS.Infrastructure.Queries.Reservations;

public class ReservationQuery(SchedulingDbContext dbContext) : IReservationQuery
{
    public async Task<Reservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await dbContext.Reservations.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyCollection<Reservation>> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken = default)
        => await dbContext.Reservations.Where(x => x.ClientId == clientId).OrderByDescending(x => x.CreatedAtUtc).ToListAsync(cancellationToken);

    public async Task<IReadOnlyCollection<Reservation>> GetByTechnicianIdAsync(Guid technicianId, CancellationToken cancellationToken = default)
    {
        var reservationIds = await dbContext.ReservationAssignments
            .Where(x => x.TechnicianId == technicianId && x.IsCurrent)
            .Select(x => x.ReservationId)
            .ToListAsync(cancellationToken);

        return await dbContext.Reservations
            .Where(x => reservationIds.Contains(x.Id))
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Reservation>> SearchAsync(ReservationOverviewFilter filter, CancellationToken cancellationToken = default)
    {
        var query = dbContext.Reservations.AsQueryable();

        if (filter.ClientId.HasValue)
        {
            query = query.Where(x => x.ClientId == filter.ClientId.Value);
        }

        if (filter.ProviderEntityId.HasValue)
        {
            query = query.Where(x => x.ProviderEntityId == filter.ProviderEntityId.Value);
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(x => x.Status == filter.Status.Value);
        }

        if (filter.FromUtc.HasValue)
        {
            query = query.Where(x => x.StartAtUtc >= filter.FromUtc.Value);
        }

        if (filter.ToUtc.HasValue)
        {
            query = query.Where(x => x.EndAtUtc <= filter.ToUtc.Value);
        }

        if (filter.TechnicianId.HasValue)
        {
            var reservationIds = await dbContext.ReservationAssignments
                .Where(x => x.TechnicianId == filter.TechnicianId.Value && x.IsCurrent)
                .Select(x => x.ReservationId)
                .ToListAsync(cancellationToken);
            query = query.Where(x => reservationIds.Contains(x.Id));
        }

        return await query
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Reservation>> GetFinalizedPendingClosureAsync(DateTime thresholdUtc, int batchSize, CancellationToken cancellationToken = default)
        => await (from reservation in dbContext.Reservations
                  join finalizedTransition in dbContext.ReservationStatusHistory
                      .Where(x => x.NewStatus == ReservationStatus.Finalized)
                      .GroupBy(x => x.ReservationId)
                       .Select(entries => new
                       {
                           ReservationId = entries.Key,
                           FinalizedAtUtc = entries.Max(x => x.ChangedAtUtc)
                       })
                    on reservation.Id equals finalizedTransition.ReservationId
                  where reservation.Status == ReservationStatus.Finalized
                        && finalizedTransition.FinalizedAtUtc <= thresholdUtc
                  orderby finalizedTransition.FinalizedAtUtc
                  select reservation)
            .Take(batchSize)
            .ToListAsync(cancellationToken);

    public async Task<bool> ExistsOverlappingActiveReservationAsync(Guid technicianId, DateTime startAtUtc, DateTime endAtUtc, Guid? excludingReservationId, CancellationToken cancellationToken = default)
    {
        var activeStatuses = new[] { ReservationStatus.Created, ReservationStatus.Approved, ReservationStatus.Confirmed, ReservationStatus.InProgress };

        var query = from assignment in dbContext.ReservationAssignments
                    join reservation in dbContext.Reservations on assignment.ReservationId equals reservation.Id
                    where assignment.TechnicianId == technicianId
                          && assignment.IsCurrent
                          && activeStatuses.Contains(reservation.Status)
                          && reservation.StartAtUtc < endAtUtc
                          && startAtUtc < reservation.EndAtUtc
                    select reservation.Id;

        if (excludingReservationId.HasValue)
            query = query.Where(x => x != excludingReservationId.Value);

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<int> CountActiveReservationsAsync(Guid technicianId, CancellationToken cancellationToken = default)
    {
        var activeStatuses = new[] { ReservationStatus.Created, ReservationStatus.Approved, ReservationStatus.Confirmed, ReservationStatus.InProgress };

        return await (from assignment in dbContext.ReservationAssignments
                      join reservation in dbContext.Reservations on assignment.ReservationId equals reservation.Id
                      where assignment.TechnicianId == technicianId
                            && assignment.IsCurrent
                            && activeStatuses.Contains(reservation.Status)
                      select reservation.Id)
            .CountAsync(cancellationToken);
    }
}


