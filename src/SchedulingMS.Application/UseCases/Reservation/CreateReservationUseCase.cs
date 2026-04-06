using Microsoft.Extensions.Logging;
using SchedulingMS.Application.DTOs.Reservation;
using SchedulingMS.Application.Events;
using SchedulingMS.Application.Exceptions;
using SchedulingMS.Application.Interfaces.Commands;
using SchedulingMS.Application.Interfaces.Ports;
using SchedulingMS.Application.Interfaces.Queries;
using SchedulingMS.Application.Interfaces.Services;
using SchedulingMS.Application.Interfaces.UseCases.Reservation;
using SchedulingMS.Domain.Entities;
using SchedulingMS.Domain.Enums;
using SchedulingMS.Domain.Utilities;

namespace SchedulingMS.Application.UseCases.Reservation;

public class CreateReservationUseCase(
    IReservationCommand reservationCommand,
    IAssignmentCommand assignmentCommand,
    IReservationStatusHistoryCommand statusHistoryCommand,
    IPreassignmentService preassignmentService,
    IEventPublisher eventPublisher,
    IUnitOfWork unitOfWork,
    ILogger<CreateReservationUseCase> logger) : ICreateReservationUseCase
{
    public async Task<ReservationResponse> ExecuteAsync(CreateReservationRequest request, CancellationToken cancellationToken = default)
    {
        var startAtUtc = ArgentinaDateTime.NormalizeToUtc(request.StartAtUtc);
        var endAtUtc = ArgentinaDateTime.NormalizeToUtc(request.EndAtUtc);

        if (request.ClientId == Guid.Empty) throw new ValidationException("ClientId is required.");
        if (request.ProviderEntityId == Guid.Empty) throw new ValidationException("ProviderEntityId is required.");
        if (request.ServiceId == Guid.Empty) throw new ValidationException("ServiceId is required.");
        if (startAtUtc >= endAtUtc) throw new ValidationException("StartAtUtc must be before EndAtUtc.");
        if (startAtUtc <= DateTime.UtcNow) throw new ValidationException("Reservation must start in the future.");

        var now = DateTime.UtcNow;
        var reservation = new Domain.Entities.Reservation(request.ClientId, request.ProviderEntityId, request.ServiceId, startAtUtc, endAtUtc);

        Domain.Entities.Reservation created = reservation;
        var assignedTechnicianId = Guid.Empty;

        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            created = await reservationCommand.CreateAsync(reservation, cancellationToken);
            assignedTechnicianId = await preassignmentService.ResolveTechnicianAsync(request.ProviderEntityId, startAtUtc, endAtUtc, cancellationToken);

            var assignment = new ReservationAssignment(created.Id, assignedTechnicianId, AssignmentType.Preassignment, now, null, "Automatic preassignment", true);
            await assignmentCommand.CreateAsync(assignment, cancellationToken);

            var history = new ReservationStatusHistory(created.Id, null, created.Status, now, null, "Reservation created");
            await statusHistoryCommand.AddAsync(history, cancellationToken);
        }, cancellationToken);

        await eventPublisher.PublishAsync(new ReservationCreatedEvent(
            created.Id,
            created.ClientId,
            created.ProviderEntityId,
            created.ServiceId,
            assignedTechnicianId,
            created.StartAtUtc,
            created.EndAtUtc,
            now), cancellationToken);

        logger.LogInformation("Reservation {ReservationId} created with technician {TechnicianId}", created.Id, assignedTechnicianId);

        return new ReservationResponse(
            created.Id,
            created.ClientId,
            created.ProviderEntityId,
            created.ServiceId,
            assignedTechnicianId,
            created.StartAtUtc,
            created.EndAtUtc,
            created.Status,
            created.CreatedAtUtc);
    }
}


