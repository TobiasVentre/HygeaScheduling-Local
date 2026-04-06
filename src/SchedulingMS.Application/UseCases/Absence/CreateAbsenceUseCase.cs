using SchedulingMS.Application.DTOs.Absence;
using SchedulingMS.Application.Exceptions;
using SchedulingMS.Application.Interfaces.Commands;
using SchedulingMS.Application.Interfaces.Queries;
using SchedulingMS.Application.Interfaces.UseCases.Absence;
using SchedulingMS.Domain.Entities;
using SchedulingMS.Domain.Utilities;

namespace SchedulingMS.Application.UseCases.Absence;

public class CreateAbsenceUseCase(IAbsenceCommand absenceCommand, IAbsenceQuery absenceQuery) : ICreateAbsenceUseCase
{
    public async Task<AbsenceResponse> ExecuteAsync(CreateAbsenceRequest request, CancellationToken cancellationToken = default)
    {
        var startAtUtc = ArgentinaDateTime.NormalizeToUtc(request.StartAtUtc);
        var endAtUtc = ArgentinaDateTime.NormalizeToUtc(request.EndAtUtc);

        if (request.TechnicianId == Guid.Empty) throw new ValidationException("TechnicianId is required.");
        if (request.ProviderEntityId == Guid.Empty) throw new ValidationException("ProviderEntityId is required.");
        if (startAtUtc >= endAtUtc) throw new ValidationException("StartAtUtc must be before EndAtUtc.");
        if (startAtUtc <= DateTime.UtcNow) throw new ValidationException("Absence must start in the future.");

        var hasOverlap = await absenceQuery.ExistsOverlapAsync(request.TechnicianId, startAtUtc, endAtUtc, null, cancellationToken);
        if (hasOverlap) throw new ConflictException("Absence overlaps an existing absence period.");

        var absence = new TechnicianAbsence(request.TechnicianId, request.ProviderEntityId, startAtUtc, endAtUtc, request.Reason);
        var created = await absenceCommand.CreateAsync(absence, cancellationToken);
        return new AbsenceResponse(created.Id, created.TechnicianId, created.ProviderEntityId, created.StartAtUtc, created.EndAtUtc, created.Reason);
    }
}


