using SchedulingMS.Domain.Entities;

namespace SchedulingMS.Application.Interfaces.Commands;

public interface IAvailabilityCommand
{
    Task<AvailabilitySlot> CreateAsync(AvailabilitySlot slot, CancellationToken cancellationToken = default);
    Task UpdateAsync(AvailabilitySlot slot, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}


