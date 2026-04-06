using SchedulingMS.Application.DTOs.Reservation;
namespace SchedulingMS.Application.Interfaces.UseCases.Reservation;
public interface IReassignReservationUseCase { Task<ReservationResponse> ExecuteAsync(Guid reservationId, ReassignReservationRequest request, CancellationToken cancellationToken = default); }


