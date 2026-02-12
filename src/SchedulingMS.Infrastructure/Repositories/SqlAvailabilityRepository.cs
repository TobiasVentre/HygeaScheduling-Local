using Microsoft.EntityFrameworkCore;
using SchedulingMS.Application.Interfaces;
using SchedulingMS.Domain.Entities;
using SchedulingMS.Infrastructure.Persistence;

namespace SchedulingMS.Infrastructure.Repositories;

public class SqlAvailabilityRepository(SchedulingDbContext dbContext) : IAvailabilityRepository
{
    public async Task<AvailabilitySlot> AddAsync(AvailabilitySlot slot, CancellationToken cancellationToken = default)
    {
        dbContext.AvailabilitySlots.Add(slot);
        await dbContext.SaveChangesAsync(cancellationToken);
        return slot;
    }

    public async Task<AvailabilitySlot?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.AvailabilitySlots
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<AvailabilitySlot>> GetByTechnicianAndRangeAsync(int technicianId, DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken = default)
    {
        return await dbContext.AvailabilitySlots
            .Where(x => x.TechnicianId == technicianId && x.StartAtUtc < toUtc && fromUtc < x.EndAtUtc)
            .OrderBy(x => x.StartAtUtc)
            .ToArrayAsync(cancellationToken);
    }

    public async Task UpdateAsync(AvailabilitySlot slot, CancellationToken cancellationToken = default)
    {
        dbContext.AvailabilitySlots.Update(slot);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var slot = await dbContext.AvailabilitySlots.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (slot is null)
        {
            return;
        }

        dbContext.AvailabilitySlots.Remove(slot);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
