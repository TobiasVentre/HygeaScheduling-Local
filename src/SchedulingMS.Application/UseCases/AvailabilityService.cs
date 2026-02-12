using SchedulingMS.Application.DTOs;
using SchedulingMS.Application.Exceptions;
using SchedulingMS.Application.Interfaces;
using SchedulingMS.Domain.Entities;

namespace SchedulingMS.Application.UseCases;

public class AvailabilityService(IAvailabilityRepository availabilityRepository) : IAvailabilityService
{
    public async Task<AvailabilityResponse> CreateAvailabilityAsync(CreateAvailabilityRequest request, CancellationToken cancellationToken = default)
    {
        ValidateRequest(request.TechnicianId, request.StartAtUtc, request.EndAtUtc);
        var slot = new AvailabilitySlot(request.TechnicianId, request.StartAtUtc, request.EndAtUtc);
        var created = await availabilityRepository.AddAsync(slot, cancellationToken);
        return Map(created);
    }

    public async Task<AvailabilityResponse> UpdateAvailabilityAsync(Guid id, UpdateAvailabilityRequest request, CancellationToken cancellationToken = default)
    {
        var slot = await availabilityRepository.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException($"Availability {id} was not found.");
        slot.Update(request.StartAtUtc, request.EndAtUtc);
        await availabilityRepository.UpdateAsync(slot, cancellationToken);
        return Map(slot);
    }

    public Task DeleteAvailabilityAsync(Guid id, CancellationToken cancellationToken = default)
        => availabilityRepository.DeleteAsync(id, cancellationToken);

    public async Task<IReadOnlyCollection<AvailabilityResponse>> GetAvailabilityByTechnicianAndRangeAsync(int technicianId, DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken = default)
    {
        ValidateRequest(technicianId, fromUtc, toUtc);
        var slots = await availabilityRepository.GetByTechnicianAndRangeAsync(technicianId, fromUtc, toUtc, cancellationToken);
        return slots.Select(Map).ToArray();
    }

    private static AvailabilityResponse Map(AvailabilitySlot slot)
        => new(slot.Id, slot.TechnicianId, slot.StartAtUtc, slot.EndAtUtc);

    private static void ValidateRequest(int technicianId, DateTime startAtUtc, DateTime endAtUtc)
    {
        if (technicianId <= 0) throw new ValidationException("TechnicianId must be greater than 0.");
        if (startAtUtc >= endAtUtc) throw new ValidationException("StartAtUtc must be before EndAtUtc.");
    }
}
