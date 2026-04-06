using SchedulingMS.Application.DTOs.Availability;
namespace SchedulingMS.Application.Interfaces.UseCases.Availability;
public interface IUpdateAvailabilityUseCase { Task<AvailabilityResponse> ExecuteAsync(Guid id, UpdateAvailabilityRequest request, CancellationToken cancellationToken = default); }


