namespace SchedulingMS.Application.DTOs;

public record CreateAvailabilityRequest(int TechnicianId, DateTime StartAtUtc, DateTime EndAtUtc);
public record UpdateAvailabilityRequest(DateTime StartAtUtc, DateTime EndAtUtc);
public record AvailabilityResponse(Guid Id, int TechnicianId, DateTime StartAtUtc, DateTime EndAtUtc);
