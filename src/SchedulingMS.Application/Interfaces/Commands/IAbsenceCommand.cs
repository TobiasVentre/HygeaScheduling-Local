using SchedulingMS.Domain.Entities;

namespace SchedulingMS.Application.Interfaces.Commands;

public interface IAbsenceCommand
{
    Task<TechnicianAbsence> CreateAsync(TechnicianAbsence absence, CancellationToken cancellationToken = default);
    Task UpdateAsync(TechnicianAbsence absence, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}


