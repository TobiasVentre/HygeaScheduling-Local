using SchedulingMS.Domain.Entities;
using SchedulingMS.Domain.Enums;

namespace SchedulingMS.Application.Interfaces;

public interface IReservationRepository
{
    Task<Reservation> AddAsync(Reservation reservation, CancellationToken cancellationToken = default);
    Task<Reservation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Reservation>> GetByClientIdAsync(int clientId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<Reservation>> GetByTechnicianIdAsync(int technicianId, CancellationToken cancellationToken = default);
    Task<bool> ExistsOverlappingReservationAsync(int technicianId, DateTime startUtc, DateTime endUtc, CancellationToken cancellationToken = default);
    Task UpdateStatusAsync(Guid id, ReservationStatus status, CancellationToken cancellationToken = default);
}
