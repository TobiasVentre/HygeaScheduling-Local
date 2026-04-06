using Microsoft.EntityFrameworkCore;
using SchedulingMS.Application.Interfaces.Queries;
using SchedulingMS.Domain.Entities;
using SchedulingMS.Infrastructure.Persistence;

namespace SchedulingMS.Infrastructure.Queries.Assignment;

public class AssignmentQuery(SchedulingDbContext dbContext) : IAssignmentQuery
{
    public async Task<ReservationAssignment?> GetCurrentByReservationIdAsync(Guid reservationId, CancellationToken cancellationToken = default)
        => await dbContext.ReservationAssignments
            .Where(x => x.ReservationId == reservationId && x.IsCurrent)
            .OrderByDescending(x => x.AssignedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);
}


