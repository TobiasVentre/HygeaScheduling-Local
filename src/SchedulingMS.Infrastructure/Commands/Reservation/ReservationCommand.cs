using Microsoft.EntityFrameworkCore;
using SchedulingMS.Application.Interfaces.Commands;
using SchedulingMS.Domain.Entities;
using SchedulingMS.Domain.Enums;
using SchedulingMS.Infrastructure.Persistence;

namespace SchedulingMS.Infrastructure.Commands.Reservations;

public class ReservationCommand(SchedulingDbContext dbContext) : IReservationCommand
{
    public async Task<Reservation> CreateAsync(Reservation reservation, CancellationToken cancellationToken = default)
    {
        await dbContext.Reservations.AddAsync(reservation, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return reservation;
    }

    public async Task UpdateStatusAsync(Guid reservationId, ReservationStatus status, CancellationToken cancellationToken = default)
    {
        var reservation = await dbContext.Reservations.FirstOrDefaultAsync(x => x.Id == reservationId, cancellationToken);
        if (reservation is null) return;

        reservation.ChangeStatus(status);
        dbContext.Reservations.Update(reservation);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteWithDependenciesAsync(Guid reservationId, CancellationToken cancellationToken = default)
    {
        var assignments = await dbContext.ReservationAssignments
            .Where(x => x.ReservationId == reservationId)
            .ToListAsync(cancellationToken);

        var history = await dbContext.ReservationStatusHistory
            .Where(x => x.ReservationId == reservationId)
            .ToListAsync(cancellationToken);

        var reservation = await dbContext.Reservations.FirstOrDefaultAsync(x => x.Id == reservationId, cancellationToken);
        if (reservation is null)
        {
            return;
        }

        dbContext.ReservationAssignments.RemoveRange(assignments);
        dbContext.ReservationStatusHistory.RemoveRange(history);
        dbContext.Reservations.Remove(reservation);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}


