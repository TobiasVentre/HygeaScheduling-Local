using SchedulingMS.Domain.Entities;

namespace SchedulingMS.Application.Interfaces.Commands;

public interface IAssignmentCommand
{
    Task<ReservationAssignment> CreateAsync(ReservationAssignment assignment, CancellationToken cancellationToken = default);
    Task MarkCurrentAsHistoricalAsync(Guid reservationId, CancellationToken cancellationToken = default);
}


