using Microsoft.EntityFrameworkCore;
using SchedulingMS.Application.Interfaces.Ports;
using SchedulingMS.Infrastructure.Persistence;

namespace SchedulingMS.Infrastructure.Ports;

public class EfUnitOfWork(SchedulingDbContext dbContext) : IUnitOfWork
{
    public async Task ExecuteInTransactionAsync(Func<Task> operation, CancellationToken cancellationToken = default)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
            await operation();
            await transaction.CommitAsync(cancellationToken);
        });
    }
}


