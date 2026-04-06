using SchedulingMS.Domain.Entities;
using SchedulingMS.Domain.Enums;

namespace SchedulingMS.Application.Interfaces.Commands;

public interface IReservationCommand
{
    Task<Reservation> CreateAsync(Reservation reservation, CancellationToken cancellationToken = default);
    Task UpdateStatusAsync(Guid reservationId, ReservationStatus status, CancellationToken cancellationToken = default);
    Task DeleteWithDependenciesAsync(Guid reservationId, CancellationToken cancellationToken = default);
}


