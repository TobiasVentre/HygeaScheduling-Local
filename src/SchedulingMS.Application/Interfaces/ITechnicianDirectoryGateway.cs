namespace SchedulingMS.Application.Interfaces;

public interface ITechnicianDirectoryGateway
{
    Task<bool> IsTechnicianApprovedAsync(int technicianId, CancellationToken cancellationToken = default);
}
