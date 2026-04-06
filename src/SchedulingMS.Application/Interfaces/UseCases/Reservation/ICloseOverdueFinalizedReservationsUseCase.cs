namespace SchedulingMS.Application.Interfaces.UseCases.Reservation;

public interface ICloseOverdueFinalizedReservationsUseCase
{
    Task<int> ExecuteAsync(CancellationToken cancellationToken = default);
}
