using SchedulingMS.Application.DTOs;

namespace SchedulingMS.Application.Interfaces;

public interface IAvailabilityService
{
    Task<AvailabilityResponse> CreateAvailabilityAsync(CreateAvailabilityRequest request, CancellationToken cancellationToken = default);
    Task<AvailabilityResponse> UpdateAvailabilityAsync(Guid id, UpdateAvailabilityRequest request, CancellationToken cancellationToken = default);
    Task DeleteAvailabilityAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<AvailabilityResponse>> GetAvailabilityByTechnicianAndRangeAsync(int technicianId, DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken = default);
}
