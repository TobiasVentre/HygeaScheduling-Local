using SchedulingMS.Application.DTOs.Reservation;
namespace SchedulingMS.Application.Interfaces.UseCases.Reservation;
public interface IGetReservationsByClientUseCase { Task<IReadOnlyCollection<ReservationResponse>> ExecuteAsync(Guid clientId, CancellationToken cancellationToken = default); }


