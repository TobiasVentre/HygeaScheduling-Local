using SchedulingMS.Application.DTOs.Reservation;
namespace SchedulingMS.Application.Interfaces.UseCases.Reservation;
public interface IUpdateReservationStatusUseCase { Task<ReservationResponse> ExecuteAsync(Guid reservationId, UpdateReservationStatusRequest request, CancellationToken cancellationToken = default); }


