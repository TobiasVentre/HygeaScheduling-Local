using SchedulingMS.Application.DTOs.Reservation;
namespace SchedulingMS.Application.Interfaces.UseCases.Reservation;
public interface ICreateReservationUseCase { Task<ReservationResponse> ExecuteAsync(CreateReservationRequest request, CancellationToken cancellationToken = default); }


