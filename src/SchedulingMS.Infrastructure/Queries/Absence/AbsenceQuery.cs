using Microsoft.EntityFrameworkCore;
using SchedulingMS.Application.Interfaces.Queries;
using SchedulingMS.Domain.Entities;
using SchedulingMS.Infrastructure.Persistence;

namespace SchedulingMS.Infrastructure.Queries.Absence;

public class AbsenceQuery(SchedulingDbContext dbContext) : IAbsenceQuery
{
    public async Task<TechnicianAbsence?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await dbContext.TechnicianAbsences.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyCollection<TechnicianAbsence>> GetByTechnicianAndRangeAsync(Guid technicianId, DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken = default)
        => await dbContext.TechnicianAbsences
            .Where(x => x.TechnicianId == technicianId && x.StartAtUtc < toUtc && fromUtc < x.EndAtUtc)
            .OrderBy(x => x.StartAtUtc)
            .ToListAsync(cancellationToken);

    public async Task<bool> ExistsOverlapAsync(Guid technicianId, DateTime startAtUtc, DateTime endAtUtc, Guid? excludingId, CancellationToken cancellationToken = default)
    {
        var query = dbContext.TechnicianAbsences
            .Where(x => x.TechnicianId == technicianId && x.StartAtUtc < endAtUtc && startAtUtc < x.EndAtUtc);

        if (excludingId.HasValue) query = query.Where(x => x.Id != excludingId.Value);
        return await query.AnyAsync(cancellationToken);
    }
}


