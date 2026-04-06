namespace SchedulingMS.Application.Interfaces.Ports;

public interface IUnitOfWork
{
    Task ExecuteInTransactionAsync(Func<Task> operation, CancellationToken cancellationToken = default);
}


