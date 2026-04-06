using SchedulingMS.Application.Interfaces.Commands;
using SchedulingMS.Domain.Entities;
using SchedulingMS.Infrastructure.Persistence;

namespace SchedulingMS.Infrastructure.Commands.History;

public class ReservationStatusHistoryCommand(SchedulingDbContext dbContext) : IReservationStatusHistoryCommand
{
    public async Task AddAsync(ReservationStatusHistory history, CancellationToken cancellationToken = default)
    {
        await dbContext.ReservationStatusHistory.AddAsync(history, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}


