using Microsoft.EntityFrameworkCore;
using SchedulingMS.Application.Interfaces.Commands;
using SchedulingMS.Domain.Entities;
using SchedulingMS.Infrastructure.Persistence;

namespace SchedulingMS.Infrastructure.Commands.Assignment;

public class AssignmentCommand(SchedulingDbContext dbContext) : IAssignmentCommand
{
    public async Task<ReservationAssignment> CreateAsync(ReservationAssignment assignment, CancellationToken cancellationToken = default)
    {
        await dbContext.ReservationAssignments.AddAsync(assignment, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
        return assignment;
    }

    public async Task MarkCurrentAsHistoricalAsync(Guid reservationId, CancellationToken cancellationToken = default)
    {
        var current = await dbContext.ReservationAssignments
            .Where(x => x.ReservationId == reservationId && x.IsCurrent)
            .ToListAsync(cancellationToken);

        foreach (var item in current) item.MarkAsHistorical();

        dbContext.ReservationAssignments.UpdateRange(current);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}


