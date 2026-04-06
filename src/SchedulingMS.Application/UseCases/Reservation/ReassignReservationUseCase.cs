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

namespace SchedulingMS.Application.UseCases.Reservation;

public class ReassignReservationUseCase(
    IReservationQuery reservationQuery,
    IAssignmentQuery assignmentQuery,
    IAssignmentCommand assignmentCommand,
    IReservationStatusHistoryCommand statusHistoryCommand,
    ITechnicianDirectoryGateway technicianDirectoryGateway,
    ISchedulingConsistencyService schedulingConsistencyService,
    IUnitOfWork unitOfWork,
    IEventPublisher eventPublisher,
    ILogger<ReassignReservationUseCase> logger) : IReassignReservationUseCase
{
    public async Task<ReservationResponse> ExecuteAsync(Guid reservationId, ReassignReservationRequest request, CancellationToken cancellationToken = default)
    {
        if (request.TechnicianId == Guid.Empty) throw new ValidationException("TechnicianId is required.");

        var reservation = await reservationQuery.GetByIdAsync(reservationId, cancellationToken)
            ?? throw new NotFoundException($"Reservation {reservationId} was not found.");

        if (reservation.Status is ReservationStatus.InProgress or ReservationStatus.Finalized or ReservationStatus.Closed)
            throw new ValidationException("Reservation cannot be reassigned in current state.");

        var current = await assignmentQuery.GetCurrentByReservationIdAsync(reservationId, cancellationToken)
            ?? throw new NotFoundException($"Reservation {reservationId} has no current assignment.");

        if (current.TechnicianId == request.TechnicianId)
            return new ReservationResponse(reservation.Id, reservation.ClientId, reservation.ProviderEntityId, reservation.ServiceId, current.TechnicianId, reservation.StartAtUtc, reservation.EndAtUtc, reservation.Status, reservation.CreatedAtUtc);

        if (request.OverrideByAdmin)
        {
            var overrideTechnician = await technicianDirectoryGateway.GetActiveTechnicianByIdAsync(request.TechnicianId, cancellationToken);
            if (overrideTechnician is null)
                throw new ValidationException("Technician is not active or does not exist.");
        }
        else
        {
            var technicians = await technicianDirectoryGateway.GetActiveTechniciansByProviderAsync(reservation.ProviderEntityId, cancellationToken);
            if (!technicians.Any(x => x.TechnicianId == request.TechnicianId))
                throw new ValidationException("Technician does not belong to reservation provider or is inactive.");
        }

        await schedulingConsistencyService.EnsureTechnicianCanTakeSlotAsync(request.TechnicianId, reservation.StartAtUtc, reservation.EndAtUtc, cancellationToken);

        var now = DateTime.UtcNow;
        var assignmentType = request.OverrideByAdmin ? AssignmentType.ManualOverride : AssignmentType.Reassignment;

        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await assignmentCommand.MarkCurrentAsHistoricalAsync(reservationId, cancellationToken);
            await assignmentCommand.CreateAsync(new ReservationAssignment(reservationId, request.TechnicianId, assignmentType, now, request.RequestedByUserId, request.Reason, true), cancellationToken);
            await statusHistoryCommand.AddAsync(new ReservationStatusHistory(reservationId, reservation.Status, reservation.Status, now, request.RequestedByUserId, $"Reassigned from {current.TechnicianId} to {request.TechnicianId}"), cancellationToken);
        }, cancellationToken);

        await eventPublisher.PublishAsync(new ReservationReassignedEvent(reservationId, current.TechnicianId, request.TechnicianId, now), cancellationToken);

        logger.LogInformation("Reservation {ReservationId} reassigned from technician {Previous} to {Current}", reservationId, current.TechnicianId, request.TechnicianId);

        return new ReservationResponse(reservation.Id, reservation.ClientId, reservation.ProviderEntityId, reservation.ServiceId, request.TechnicianId, reservation.StartAtUtc, reservation.EndAtUtc, reservation.Status, reservation.CreatedAtUtc);
    }
}


