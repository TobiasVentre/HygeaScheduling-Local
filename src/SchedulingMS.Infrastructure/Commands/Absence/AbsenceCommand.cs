using Microsoft.EntityFrameworkCore;
using SchedulingMS.Application.Interfaces.Commands;
using SchedulingMS.Domain.Entities;
using SchedulingMS.Infrastructure.Persistence;

namespace SchedulingMS.Infrastructure.Commands.Absence;

public class AbsenceCommand(SchedulingDbContext dbContext) : IAbsenceCommand
{
    public async Task<TechnicianAbsence> CreateAsync(TechnicianAbsence absence, CancellationToken cancellationToken = default)
    {
        await dbContext.TechnicianAbsences.AddAsync(absence, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return absence;
    }

    public async Task UpdateAsync(TechnicianAbsence absence, CancellationToken cancellationToken = default)
    {
        dbContext.TechnicianAbsences.Update(absence);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await dbContext.TechnicianAbsences.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null) return;
        dbContext.TechnicianAbsences.Remove(entity);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}


