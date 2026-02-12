using SchedulingMS.Application.DTOs;

namespace SchedulingMS.Application.Interfaces;

public interface IReservationService
{
    Task<ReservationResponse> CreateReservationAsync(CreateReservationRequest request, CancellationToken cancellationToken = default);
    Task<ReservationResponse?> GetByIdAsync(Guid reservationId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ReservationResponse>> GetByClientIdAsync(int clientId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ReservationResponse>> GetByTechnicianIdAsync(int technicianId, CancellationToken cancellationToken = default);
    Task<ReservationResponse> UpdateStatusAsync(Guid reservationId, UpdateReservationStatusRequest request, CancellationToken cancellationToken = default);
}
