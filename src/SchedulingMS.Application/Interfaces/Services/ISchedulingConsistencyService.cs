namespace SchedulingMS.Application.Interfaces.Services;

public interface ISchedulingConsistencyService
{
    Task EnsureTechnicianCanTakeSlotAsync(Guid technicianId, DateTime startAtUtc, DateTime endAtUtc, CancellationToken cancellationToken = default);
}


