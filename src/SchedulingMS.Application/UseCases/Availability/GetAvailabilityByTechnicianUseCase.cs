using SchedulingMS.Application.DTOs.Availability;
using SchedulingMS.Application.Exceptions;
using SchedulingMS.Application.Interfaces.Queries;
using SchedulingMS.Application.Interfaces.UseCases.Availability;
using SchedulingMS.Domain.Utilities;

namespace SchedulingMS.Application.UseCases.Availability;

public class GetAvailabilityByTechnicianUseCase(IAvailabilityQuery availabilityQuery) : IGetAvailabilityByTechnicianUseCase
{
    public async Task<IReadOnlyCollection<AvailabilityResponse>> ExecuteAsync(Guid technicianId, DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken = default)
    {
        fromUtc = ArgentinaDateTime.NormalizeToUtc(fromUtc);
        toUtc = ArgentinaDateTime.NormalizeToUtc(toUtc);

        if (technicianId == Guid.Empty) throw new ValidationException("TechnicianId is required.");
        if (fromUtc >= toUtc) throw new ValidationException("fromUtc must be before toUtc.");

        var slots = await availabilityQuery.GetByTechnicianAndRangeAsync(technicianId, fromUtc, toUtc, cancellationToken);
        return slots.Select(x => new AvailabilityResponse(x.Id, x.TechnicianId, x.ProviderEntityId, x.StartAtUtc, x.EndAtUtc)).ToArray();
    }
}


