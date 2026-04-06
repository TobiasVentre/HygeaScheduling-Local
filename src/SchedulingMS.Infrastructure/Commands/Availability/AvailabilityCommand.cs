using Microsoft.EntityFrameworkCore;
using SchedulingMS.Application.Interfaces.Commands;
using SchedulingMS.Domain.Entities;
using SchedulingMS.Infrastructure.Persistence;

namespace SchedulingMS.Infrastructure.Commands.Availability;

public class AvailabilityCommand(SchedulingDbContext dbContext) : IAvailabilityCommand
{
    public async Task<AvailabilitySlot> CreateAsync(AvailabilitySlot slot, CancellationToken cancellationToken = default)
    {
        await dbContext.AvailabilitySlots.AddAsync(slot, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return slot;
    }

    public async Task UpdateAsync(AvailabilitySlot slot, CancellationToken cancellationToken = default)
    {
        dbContext.AvailabilitySlots.Update(slot);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.AvailabilitySlots.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return;
        dbContext.AvailabilitySlots.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}


