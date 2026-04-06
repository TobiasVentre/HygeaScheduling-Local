using SchedulingMS.Application.DTOs.Absence;
using SchedulingMS.Application.Exceptions;
using SchedulingMS.Application.Interfaces.Queries;
using SchedulingMS.Application.Interfaces.UseCases.Absence;
using SchedulingMS.Domain.Utilities;

namespace SchedulingMS.Application.UseCases.Absence;

public class GetAbsencesByTechnicianUseCase(IAbsenceQuery absenceQuery) : IGetAbsencesByTechnicianUseCase
{
    public async Task<IReadOnlyCollection<AbsenceResponse>> ExecuteAsync(Guid technicianId, DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken = default)
    {
        fromUtc = ArgentinaDateTime.NormalizeToUtc(fromUtc);
        toUtc = ArgentinaDateTime.NormalizeToUtc(toUtc);

        if (technicianId == Guid.Empty) throw new ValidationException("TechnicianId is required.");
        if (fromUtc >= toUtc) throw new ValidationException("fromUtc must be before toUtc.");

        var absences = await absenceQuery.GetByTechnicianAndRangeAsync(technicianId, fromUtc, toUtc, cancellationToken);
        return absences.Select(x => new AbsenceResponse(x.Id, x.TechnicianId, x.ProviderEntityId, x.StartAtUtc, x.EndAtUtc, x.Reason)).ToArray();
    }
}


