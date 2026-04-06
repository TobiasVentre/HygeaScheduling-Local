using SchedulingMS.Application.DTOs.Absence;
using SchedulingMS.Application.Exceptions;
using SchedulingMS.Application.Interfaces.Commands;
using SchedulingMS.Application.Interfaces.Queries;
using SchedulingMS.Application.Interfaces.UseCases.Absence;
using SchedulingMS.Domain.Utilities;

namespace SchedulingMS.Application.UseCases.Absence;

public class UpdateAbsenceUseCase(IAbsenceCommand absenceCommand, IAbsenceQuery absenceQuery) : IUpdateAbsenceUseCase
{
    public async Task<AbsenceResponse> ExecuteAsync(Guid id, UpdateAbsenceRequest request, CancellationToken cancellationToken = default)
    {
        var startAtUtc = ArgentinaDateTime.NormalizeToUtc(request.StartAtUtc);
        var endAtUtc = ArgentinaDateTime.NormalizeToUtc(request.EndAtUtc);

        if (startAtUtc >= endAtUtc) throw new ValidationException("StartAtUtc must be before EndAtUtc.");
        if (endAtUtc <= DateTime.UtcNow) throw new ValidationException("Absence cannot end in the past.");

        var absence = await absenceQuery.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException($"Absence {id} was not found.");
        absence.Update(startAtUtc, endAtUtc, request.Reason);

        var hasOverlap = await absenceQuery.ExistsOverlapAsync(absence.TechnicianId, absence.StartAtUtc, absence.EndAtUtc, id, cancellationToken);
        if (hasOverlap) throw new ConflictException("Absence overlaps an existing absence period.");

        await absenceCommand.UpdateAsync(absence, cancellationToken);
        return new AbsenceResponse(absence.Id, absence.TechnicianId, absence.ProviderEntityId, absence.StartAtUtc, absence.EndAtUtc, absence.Reason);
    }
}


