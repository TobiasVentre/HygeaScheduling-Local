using SchedulingMS.Application.DTOs.Absence;
namespace SchedulingMS.Application.Interfaces.UseCases.Absence;
public interface IUpdateAbsenceUseCase { Task<AbsenceResponse> ExecuteAsync(Guid id, UpdateAbsenceRequest request, CancellationToken cancellationToken = default); }


