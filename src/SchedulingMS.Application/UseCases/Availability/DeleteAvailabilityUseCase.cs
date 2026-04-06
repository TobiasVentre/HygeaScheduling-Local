using SchedulingMS.Application.Interfaces.Commands;
using SchedulingMS.Application.Interfaces.UseCases.Availability;

namespace SchedulingMS.Application.UseCases.Availability;

public class DeleteAvailabilityUseCase(IAvailabilityCommand availabilityCommand) : IDeleteAvailabilityUseCase
{
    public Task ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
        => availabilityCommand.DeleteAsync(id, cancellationToken);
}


