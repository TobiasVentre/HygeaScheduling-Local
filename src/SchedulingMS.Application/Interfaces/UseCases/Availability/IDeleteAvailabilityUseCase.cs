namespace SchedulingMS.Application.Interfaces.UseCases.Availability;
public interface IDeleteAvailabilityUseCase { Task ExecuteAsync(Guid id, CancellationToken cancellationToken = default); }


