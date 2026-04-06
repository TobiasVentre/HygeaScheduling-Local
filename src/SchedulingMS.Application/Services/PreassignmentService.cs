using SchedulingMS.Application.Exceptions;
using SchedulingMS.Application.Interfaces.Ports;
using SchedulingMS.Application.Interfaces.Queries;
using SchedulingMS.Application.Interfaces.Services;

namespace SchedulingMS.Application.Services;

public class PreassignmentService(
    ITechnicianDirectoryGateway technicianDirectoryGateway,
    ISchedulingConsistencyService schedulingConsistencyService,
    IReservationQuery reservationQuery) : IPreassignmentService
{
    public async Task<Guid> ResolveTechnicianAsync(Guid providerEntityId, DateTime startAtUtc, DateTime endAtUtc, CancellationToken cancellationToken = default)
    {
        if (providerEntityId == Guid.Empty) throw new ValidationException("ProviderEntityId is required.");

        var technicians = await technicianDirectoryGateway.GetActiveTechniciansByProviderAsync(providerEntityId, cancellationToken);
        if (technicians.Count == 0)
            throw new ValidationException("No active technicians found for provider.");

        var ranked = new List<(Guid technicianId, int load)>();

        foreach (var technician in technicians)
        {
            try
            {
                await schedulingConsistencyService.EnsureTechnicianCanTakeSlotAsync(technician.TechnicianId, startAtUtc, endAtUtc, cancellationToken);
                var load = await reservationQuery.CountActiveReservationsAsync(technician.TechnicianId, cancellationToken);
                ranked.Add((technician.TechnicianId, load));
            }
            catch
            {
                // Technician not eligible for requested slot.
            }
        }

        if (ranked.Count == 0)
            throw new ValidationException("No available technicians for requested slot.");

        return ranked.OrderBy(x => x.load).ThenBy(x => x.technicianId).First().technicianId;
    }
}


