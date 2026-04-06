namespace SchedulingMS.Application.Interfaces.UseCases.Absence;
public interface IDeleteAbsenceUseCase { Task ExecuteAsync(Guid id, CancellationToken cancellationToken = default); }


