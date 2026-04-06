namespace SchedulingMS.Application.DTOs.Absence;

public record CreateAbsenceRequest(Guid TechnicianId, Guid ProviderEntityId, DateTime StartAtUtc, DateTime EndAtUtc, string Reason);
public record UpdateAbsenceRequest(DateTime StartAtUtc, DateTime EndAtUtc, string Reason);
public record AbsenceResponse(Guid Id, Guid TechnicianId, Guid ProviderEntityId, DateTime StartAtUtc, DateTime EndAtUtc, string Reason);


