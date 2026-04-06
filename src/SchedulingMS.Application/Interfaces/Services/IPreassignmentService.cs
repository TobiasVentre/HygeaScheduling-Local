namespace SchedulingMS.Application.Interfaces.Services;

public interface IPreassignmentService
{
    Task<Guid> ResolveTechnicianAsync(Guid providerEntityId, DateTime startAtUtc, DateTime endAtUtc, CancellationToken cancellationToken = default);
}


