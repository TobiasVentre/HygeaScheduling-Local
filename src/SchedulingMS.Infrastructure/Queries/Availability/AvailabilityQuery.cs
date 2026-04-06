using Microsoft.EntityFrameworkCore;
using SchedulingMS.Application.Interfaces.Queries;
using SchedulingMS.Domain.Entities;
using SchedulingMS.Infrastructure.Persistence;

namespace SchedulingMS.Infrastructure.Queries.Availability;

public class AvailabilityQuery(SchedulingDbContext dbContext) : IAvailabilityQuery
{
    public async Task<AvailabilitySlot?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await dbContext.AvailabilitySlots.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyCollection<AvailabilitySlot>> GetByTechnicianAndRangeAsync(Guid technicianId, DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken = default)
        => await dbContext.AvailabilitySlots
            .Where(x => x.TechnicianId == technicianId && x.StartAtUtc < toUtc && fromUtc < x.EndAtUtc)
            .OrderBy(x => x.StartAtUtc)
            .ToListAsync(cancellationToken);

    public async Task<bool> HasCoveringSlotAsync(Guid technicianId, DateTime startAtUtc, DateTime endAtUtc, CancellationToken cancellationToken = default)
        => await dbContext.AvailabilitySlots
            .AnyAsync(x => x.TechnicianId == technicianId && x.StartAtUtc <= startAtUtc && x.EndAtUtc >= endAtUtc, cancellationToken);

    public async Task<bool> ExistsOverlapAsync(Guid technicianId, DateTime startAtUtc, DateTime endAtUtc, Guid? excludingId, CancellationToken cancellationToken = default)
    {
        var query = dbContext.AvailabilitySlots
            .Where(x => x.TechnicianId == technicianId && x.StartAtUtc < endAtUtc && startAtUtc < x.EndAtUtc);

        if (excludingId.HasValue) query = query.Where(x => x.Id != excludingId.Value);
        return await query.AnyAsync(cancellationToken);
    }
}


