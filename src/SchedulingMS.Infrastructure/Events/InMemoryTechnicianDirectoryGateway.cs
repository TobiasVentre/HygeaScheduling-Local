using SchedulingMS.Application.Interfaces;

namespace SchedulingMS.Infrastructure.Events;

public class InMemoryTechnicianDirectoryGateway : ITechnicianDirectoryGateway
{
    private static readonly HashSet<int> ApprovedTechnicians = [1, 2, 3, 4, 5, 10, 20];

    public Task<bool> IsTechnicianApprovedAsync(int technicianId, CancellationToken cancellationToken = default)
        => Task.FromResult(ApprovedTechnicians.Contains(technicianId));
}
