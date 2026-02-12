using Microsoft.EntityFrameworkCore;
using SchedulingMS.Application.Interfaces;
using SchedulingMS.Domain.Entities;
using SchedulingMS.Domain.Enums;
using SchedulingMS.Infrastructure.Persistence;

namespace SchedulingMS.Infrastructure.Repositories;

public class SqlReservationRepository(SchedulingDbContext dbContext) : IReservationRepository
{
    public async Task<Reservation> AddAsync(Reservation reservation, CancellationToken cancellationToken = default)
    {
        dbContext.Reservations.Add(reservation);
        await dbContext.SaveChangesAsync(cancellationToken);
        return reservation;
    }

    public async Task<Reservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Reservations
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<Reservation>> GetByClientIdAsync(int clientId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Reservations
            .Where(x => x.ClientId == clientId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<Reservation>> GetByTechnicianIdAsync(int technicianId, CancellationToken cancellationToken = default)
    {
        return await dbContext.Reservations
            .Where(x => x.TechnicianId == technicianId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToArrayAsync(cancellationToken);
    }

    public async Task<bool> ExistsOverlappingReservationAsync(int technicianId, DateTime startUtc, DateTime endUtc, CancellationToken cancellationToken = default)
    {
        return await dbContext.Reservations.AnyAsync(x =>
                x.TechnicianId == technicianId
                && x.Status != ReservationStatus.Cancelled
                && x.StartAtUtc < endUtc
                && startUtc < x.EndAtUtc,
            cancellationToken);
    }

    public async Task UpdateStatusAsync(Guid id, ReservationStatus status, CancellationToken cancellationToken = default)
    {
        var reservation = await dbContext.Reservations.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (reservation is null)
        {
            return;
        }

        reservation.UpdateStatus(status);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
