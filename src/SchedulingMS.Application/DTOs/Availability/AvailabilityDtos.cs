namespace SchedulingMS.Application.DTOs.Availability;

public record CreateAvailabilityRequest(Guid TechnicianId, Guid ProviderEntityId, DateTime StartAtUtc, DateTime EndAtUtc);
public record UpdateAvailabilityRequest(DateTime StartAtUtc, DateTime EndAtUtc);
public record AvailabilityResponse(Guid Id, Guid TechnicianId, Guid ProviderEntityId, DateTime StartAtUtc, DateTime EndAtUtc);


