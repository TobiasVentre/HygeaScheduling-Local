using SchedulingMS.Domain.Entities;

namespace SchedulingMS.Application.Interfaces.Queries;

public interface IAssignmentQuery
{
    Task<ReservationAssignment?> GetCurrentByReservationIdAsync(Guid reservationId, CancellationToken cancellationToken = default);
}


