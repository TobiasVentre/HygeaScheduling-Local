using SchedulingMS.Application.DTOs.Absence;
namespace SchedulingMS.Application.Interfaces.UseCases.Absence;
public interface IGetAbsencesByTechnicianUseCase { Task<IReadOnlyCollection<AbsenceResponse>> ExecuteAsync(Guid technicianId, DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken = default); }


