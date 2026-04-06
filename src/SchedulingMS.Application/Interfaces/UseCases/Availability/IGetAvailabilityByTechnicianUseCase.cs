using SchedulingMS.Application.DTOs.Availability;
namespace SchedulingMS.Application.Interfaces.UseCases.Availability;
public interface IGetAvailabilityByTechnicianUseCase { Task<IReadOnlyCollection<AvailabilityResponse>> ExecuteAsync(Guid technicianId, DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken = default); }


