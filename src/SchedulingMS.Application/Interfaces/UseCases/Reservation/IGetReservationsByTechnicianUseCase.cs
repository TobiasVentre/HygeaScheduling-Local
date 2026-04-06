using SchedulingMS.Application.DTOs.Reservation;
namespace SchedulingMS.Application.Interfaces.UseCases.Reservation;
public interface IGetReservationsByTechnicianUseCase { Task<IReadOnlyCollection<ReservationResponse>> ExecuteAsync(Guid technicianId, CancellationToken cancellationToken = default); }


