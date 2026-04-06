using SchedulingMS.Application.Interfaces.Commands;
using SchedulingMS.Application.Interfaces.UseCases.Absence;

namespace SchedulingMS.Application.UseCases.Absence;

public class DeleteAbsenceUseCase(IAbsenceCommand absenceCommand) : IDeleteAbsenceUseCase
{
    public Task ExecuteAsync(Guid id, CancellationToken cancellationToken = default)
        => absenceCommand.DeleteAsync(id, cancellationToken);
}


