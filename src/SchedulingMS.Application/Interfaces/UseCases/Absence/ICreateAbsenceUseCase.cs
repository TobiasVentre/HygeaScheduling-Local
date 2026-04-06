using SchedulingMS.Application.DTOs.Absence;
namespace SchedulingMS.Application.Interfaces.UseCases.Absence;
public interface ICreateAbsenceUseCase { Task<AbsenceResponse> ExecuteAsync(CreateAbsenceRequest request, CancellationToken cancellationToken = default); }


