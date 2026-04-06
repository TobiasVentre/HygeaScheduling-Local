using SchedulingMS.Domain.Entities;

namespace SchedulingMS.Application.Interfaces.Commands;

public interface IReservationStatusHistoryCommand
{
    Task AddAsync(ReservationStatusHistory history, CancellationToken cancellationToken = default);
}


