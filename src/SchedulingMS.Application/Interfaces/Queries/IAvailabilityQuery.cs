using SchedulingMS.Domain.Entities;

namespace SchedulingMS.Application.Interfaces.Queries;

public interface IAvailabilityQuery
{
    Task<AvailabilitySlot?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<AvailabilitySlot>> GetByTechnicianAndRangeAsync(Guid technicianId, DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken = default);
    Task<bool> HasCoveringSlotAsync(Guid technicianId, DateTime startAtUtc, DateTime endAtUtc, CancellationToken cancellationToken = default);
    Task<bool> ExistsOverlapAsync(Guid technicianId, DateTime startAtUtc, DateTime endAtUtc, Guid? excludingId, CancellationToken cancellationToken = default);
}


