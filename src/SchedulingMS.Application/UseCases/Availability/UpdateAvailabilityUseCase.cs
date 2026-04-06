using SchedulingMS.Application.DTOs.Availability;
using SchedulingMS.Application.Exceptions;
using SchedulingMS.Application.Interfaces.Commands;
using SchedulingMS.Application.Interfaces.Queries;
using SchedulingMS.Application.Interfaces.UseCases.Availability;
using SchedulingMS.Domain.Utilities;

namespace SchedulingMS.Application.UseCases.Availability;

public class UpdateAvailabilityUseCase(IAvailabilityCommand availabilityCommand, IAvailabilityQuery availabilityQuery) : IUpdateAvailabilityUseCase
{
    public async Task<AvailabilityResponse> ExecuteAsync(Guid id, UpdateAvailabilityRequest request, CancellationToken cancellationToken = default)
    {
        var startAtUtc = ArgentinaDateTime.NormalizeToUtc(request.StartAtUtc);
        var endAtUtc = ArgentinaDateTime.NormalizeToUtc(request.EndAtUtc);

        if (startAtUtc >= endAtUtc) throw new ValidationException("StartAtUtc must be before EndAtUtc.");
        if (endAtUtc <= DateTime.UtcNow) throw new ValidationException("Availability cannot end in the past.");

        var slot = await availabilityQuery.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException($"Availability {id} was not found.");
        slot.Update(startAtUtc, endAtUtc);

        var hasOverlap = await availabilityQuery.ExistsOverlapAsync(slot.TechnicianId, slot.StartAtUtc, slot.EndAtUtc, id, cancellationToken);
        if (hasOverlap) throw new ConflictException("Availability overlaps an existing slot.");

        await availabilityCommand.UpdateAsync(slot, cancellationToken);
        return new AvailabilityResponse(slot.Id, slot.TechnicianId, slot.ProviderEntityId, slot.StartAtUtc, slot.EndAtUtc);
    }
}


