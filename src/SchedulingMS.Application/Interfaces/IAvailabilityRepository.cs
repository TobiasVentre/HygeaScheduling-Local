using SchedulingMS.Domain.Entities;

namespace SchedulingMS.Application.Interfaces;

public interface IAvailabilityRepository
{
    Task<AvailabilitySlot> AddAsync(AvailabilitySlot slot, CancellationToken cancellationToken = default);
    Task<AvailabilitySlot?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<AvailabilitySlot>> GetByTechnicianAndRangeAsync(int technicianId, DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken = default);
    Task UpdateAsync(AvailabilitySlot slot, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
