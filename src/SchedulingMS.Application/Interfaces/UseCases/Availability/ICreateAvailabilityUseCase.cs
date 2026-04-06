using SchedulingMS.Application.DTOs.Availability;
namespace SchedulingMS.Application.Interfaces.UseCases.Availability;
public interface ICreateAvailabilityUseCase { Task<AvailabilityResponse> ExecuteAsync(CreateAvailabilityRequest request, CancellationToken cancellationToken = default); }


