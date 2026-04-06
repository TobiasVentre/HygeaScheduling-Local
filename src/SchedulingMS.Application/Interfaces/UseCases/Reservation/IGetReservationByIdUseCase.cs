using SchedulingMS.Application.DTOs.Reservation;
namespace SchedulingMS.Application.Interfaces.UseCases.Reservation;
public interface IGetReservationByIdUseCase { Task<ReservationResponse?> ExecuteAsync(Guid reservationId, CancellationToken cancellationToken = default); }


