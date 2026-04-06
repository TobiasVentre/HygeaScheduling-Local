using SchedulingMS.Application.Exceptions;
using SchedulingMS.Application.Interfaces.Queries;
using SchedulingMS.Application.Interfaces.Services;

namespace SchedulingMS.Application.Services;

public class SchedulingConsistencyService(
    IAvailabilityQuery availabilityQuery,
    IAbsenceQuery absenceQuery,
    IReservationQuery reservationQuery) : ISchedulingConsistencyService
{
    public async Task EnsureTechnicianCanTakeSlotAsync(Guid technicianId, DateTime startAtUtc, DateTime endAtUtc, CancellationToken cancellationToken = default)
    {
        var hasCoveringAvailability = await availabilityQuery.HasCoveringSlotAsync(technicianId, startAtUtc, endAtUtc, cancellationToken);
        if (!hasCoveringAvailability)
            throw new ValidationException("Technician has no availability for requested slot.");

        var hasAbsence = await absenceQuery.ExistsOverlapAsync(technicianId, startAtUtc, endAtUtc, null, cancellationToken);
        if (hasAbsence)
            throw new ConflictException("Technician is absent during requested slot.");

        var hasOverlap = await reservationQuery.ExistsOverlappingActiveReservationAsync(technicianId, startAtUtc, endAtUtc, null, cancellationToken);
        if (hasOverlap)
            throw new ConflictException("Technician already has an active reservation in requested slot.");
    }
}


