namespace SchedulingMS.Application.Interfaces.Ports;

public record TechnicianSummary(Guid TechnicianId, Guid ProviderEntityId, bool IsActive);

public interface ITechnicianDirectoryGateway
{
    Task<IReadOnlyCollection<TechnicianSummary>> GetActiveTechniciansByProviderAsync(Guid providerEntityId, CancellationToken cancellationToken = default);
    Task<TechnicianSummary?> GetActiveTechnicianByIdAsync(Guid technicianId, CancellationToken cancellationToken = default);
}


