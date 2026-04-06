using SchedulingMS.Application.DTOs.Availability;
using SchedulingMS.Application.Exceptions;
using SchedulingMS.Application.Interfaces.Commands;
using SchedulingMS.Application.Interfaces.Queries;
using SchedulingMS.Application.Interfaces.UseCases.Availability;
using SchedulingMS.Domain.Entities;
using SchedulingMS.Domain.Utilities;

namespace SchedulingMS.Application.UseCases.Availability;

public class CreateAvailabilityUseCase(IAvailabilityCommand availabilityCommand, IAvailabilityQuery availabilityQuery) : ICreateAvailabilityUseCase
{
    public async Task<AvailabilityResponse> ExecuteAsync(CreateAvailabilityRequest request, CancellationToken cancellationToken = default)
    {
        var startAtUtc = ArgentinaDateTime.NormalizeToUtc(request.StartAtUtc);
        var endAtUtc = ArgentinaDateTime.NormalizeToUtc(request.EndAtUtc);

        if (request.TechnicianId == Guid.Empty) throw new ValidationException("TechnicianId is required.");
        if (request.ProviderEntityId == Guid.Empty) throw new ValidationException("ProviderEntityId is required.");
        if (startAtUtc >= endAtUtc) throw new ValidationException("StartAtUtc must be before EndAtUtc.");
        if (startAtUtc <= DateTime.UtcNow) throw new ValidationException("Availability must start in the future.");

        var hasOverlap = await availabilityQuery.ExistsOverlapAsync(request.TechnicianId, startAtUtc, endAtUtc, null, cancellationToken);
        if (hasOverlap) throw new ConflictException("Availability overlaps an existing slot.");

        var slot = new AvailabilitySlot(request.TechnicianId, request.ProviderEntityId, startAtUtc, endAtUtc);
        var created = await availabilityCommand.CreateAsync(slot, cancellationToken);
        return new AvailabilityResponse(created.Id, created.TechnicianId, created.ProviderEntityId, created.StartAtUtc, created.EndAtUtc);
    }
}


