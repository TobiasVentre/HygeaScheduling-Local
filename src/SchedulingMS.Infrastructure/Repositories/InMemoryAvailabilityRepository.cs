using SchedulingMS.Application.Interfaces;
using SchedulingMS.Domain.Entities;

namespace SchedulingMS.Infrastructure.Repositories;

public class InMemoryAvailabilityRepository : IAvailabilityRepository
{
    private static readonly List<AvailabilitySlot> Storage = [];
    private static readonly object SyncLock = new();

    public Task<AvailabilitySlot> AddAsync(AvailabilitySlot slot, CancellationToken cancellationToken = default)
    {
        lock (SyncLock)
        {
            Storage.Add(slot);
        }

        return Task.FromResult(slot);
    }

    public Task<AvailabilitySlot?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        AvailabilitySlot? slot;
        lock (SyncLock)
        {
            slot = Storage.FirstOrDefault(x => x.Id == id);
        }

        return Task.FromResult(slot);
    }

    public Task<IReadOnlyCollection<AvailabilitySlot>> GetByTechnicianAndRangeAsync(int technicianId, DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken = default)
    {
        IReadOnlyCollection<AvailabilitySlot> result;
        lock (SyncLock)
        {
            result = Storage
                .Where(x => x.TechnicianId == technicianId && x.StartAtUtc < toUtc && fromUtc < x.EndAtUtc)
                .ToArray();
        }

        return Task.FromResult(result);
    }

    public Task UpdateAsync(AvailabilitySlot slot, CancellationToken cancellationToken = default)
    {
        lock (SyncLock)
        {
            var index = Storage.FindIndex(x => x.Id == slot.Id);
            if (index >= 0)
            {
                Storage[index] = slot;
            }
        }

        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        lock (SyncLock)
        {
            Storage.RemoveAll(x => x.Id == id);
        }

        return Task.CompletedTask;
    }
}
