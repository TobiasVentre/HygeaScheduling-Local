using SchedulingMS.Domain.Entities;
using SchedulingMS.Application.DTOs.Reservation;

namespace SchedulingMS.Application.Interfaces.Queries;

public interface IReservationQuery
{
    Task<Reservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Reservation>> GetByClientIdAsync(Guid clientId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Reservation>> GetByTechnicianIdAsync(Guid technicianId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Reservation>> SearchAsync(ReservationOverviewFilter filter, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Reservation>> GetFinalizedPendingClosureAsync(DateTime thresholdUtc, int batchSize, CancellationToken cancellationToken = default);
    Task<bool> ExistsOverlappingActiveReservationAsync(Guid technicianId, DateTime startAtUtc, DateTime endAtUtc, Guid? excludingReservationId, CancellationToken cancellationToken = default);
    Task<int> CountActiveReservationsAsync(Guid technicianId, CancellationToken cancellationToken = default);
}


