using SchedulingMS.Application.Interfaces;
using SchedulingMS.Domain.Entities;
using SchedulingMS.Domain.Enums;

namespace SchedulingMS.Infrastructure.Repositories;

public class InMemoryReservationRepository : IReservationRepository
{
    private static readonly List<Reservation> Storage = [];
    private static readonly object SyncLock = new();

    public Task<Reservation> AddAsync(Reservation reservation, CancellationToken cancellationToken = default)
    {
        lock (SyncLock)
        {
            Storage.Add(reservation);
        }

        return Task.FromResult(reservation);
    }

    public Task<Reservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        Reservation? result;
        lock (SyncLock)
        {
            result = Storage.FirstOrDefault(x => x.Id == id);
        }

        return Task.FromResult(result);
    }

    public Task<IReadOnlyCollection<Reservation>> GetByClientIdAsync(int clientId, CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<Reservation> result;
        lock (SyncLock)
        {
            result = Storage
                .Where(x => x.ClientId == clientId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .ToArray();
        }

        return Task.FromResult(result);
    }

    public Task<IReadOnlyCollection<Reservation>> GetByTechnicianIdAsync(int technicianId, CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<Reservation> result;
        lock (SyncLock)
        {
            result = Storage
                .Where(x => x.TechnicianId == technicianId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .ToArray();
        }

        return Task.FromResult(result);
    }

    public Task<bool> ExistsOverlappingReservationAsync(int technicianId, DateTime startUtc, DateTime endUtc, CancellationToken cancellationToken = default)
    {
        bool exists;
        lock (SyncLock)
        {
            exists = Storage.Any(x => x.TechnicianId == technicianId
                                      && x.Status != ReservationStatus.Cancelled
                                      && x.Overlaps(startUtc, endUtc));
        }

        return Task.FromResult(exists);
    }

    public Task UpdateStatusAsync(Guid id, ReservationStatus status, CancellationToken cancellationToken = default)
    {
        lock (SyncLock)
        {
            var reservation = Storage.FirstOrDefault(x => x.Id == id);
            reservation?.UpdateStatus(status);
        }

        return Task.CompletedTask;
    }
}
