using SchedulingMS.Domain.Entities;

namespace SchedulingMS.Application.Interfaces.Queries;

public interface IAbsenceQuery
{
    Task<TechnicianAbsence?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<TechnicianAbsence>> GetByTechnicianAndRangeAsync(Guid technicianId, DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken = default);
    Task<bool> ExistsOverlapAsync(Guid technicianId, DateTime startAtUtc, DateTime endAtUtc, Guid? excludingId, CancellationToken cancellationToken = default);
}


