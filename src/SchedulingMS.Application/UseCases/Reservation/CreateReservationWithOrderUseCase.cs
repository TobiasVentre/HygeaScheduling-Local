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

public class CreateReservationWithOrderUseCase(
    IReservationCommand reservationCommand,
    IAssignmentCommand assignmentCommand,
    IReservationStatusHistoryCommand statusHistoryCommand,
    IPreassignmentService preassignmentService,
    IOrderCreationGateway orderCreationGateway,
    IEventPublisher eventPublisher,
    IUnitOfWork unitOfWork,
    ILogger<CreateReservationWithOrderUseCase> logger) : ICreateReservationWithOrderUseCase
{
    public async Task<ReservationOrderResponse> ExecuteAsync(CreateReservationWithOrderRequest request, CancellationToken cancellationToken = default)
    {
        if (request.ClientId == Guid.Empty) throw new ValidationException("ClientId is required.");
        if (request.ProviderEntityId == Guid.Empty) throw new ValidationException("ProviderEntityId is required.");
        if (request.Items is null || request.Items.Count == 0) throw new ValidationException("At least one order item is required.");

        foreach (var item in request.Items)
        {
            if (item.ServiceId == Guid.Empty) throw new ValidationException("ServiceId is required for each item.");
            if (string.IsNullOrWhiteSpace(item.ServiceName)) throw new ValidationException("ServiceName is required for each item.");
            if (item.UnitPrice <= 0) throw new ValidationException("UnitPrice must be greater than zero for each item.");
            if (item.Quantity <= 0) throw new ValidationException("Quantity must be greater than zero for each item.");
            if (item.DurationMinutes <= 0) throw new ValidationException("DurationMinutes must be greater than zero for each item.");
        }

        var startAtUtc = ArgentinaDateTime.NormalizeToUtc(request.StartAtUtc);
        var totalDurationMinutes = request.Items.Sum(x => x.DurationMinutes * x.Quantity);
        var endAtUtc = startAtUtc.AddMinutes(totalDurationMinutes);
        var primaryServiceId = request.Items.First().ServiceId;
        var now = DateTime.UtcNow;

        if (startAtUtc <= now)
        {
            throw new ValidationException("Reservation must start in the future.");
        }

        var reservation = new Domain.Entities.Reservation(
            request.ClientId,
            request.ProviderEntityId,
            primaryServiceId,
            startAtUtc,
            endAtUtc);

        Domain.Entities.Reservation createdReservation = reservation;
        Guid assignedTechnicianId = Guid.Empty;

        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            createdReservation = await reservationCommand.CreateAsync(reservation, cancellationToken);
            assignedTechnicianId = await preassignmentService.ResolveTechnicianAsync(request.ProviderEntityId, startAtUtc, endAtUtc, cancellationToken);

            var assignment = new ReservationAssignment(createdReservation.Id, assignedTechnicianId, AssignmentType.Preassignment, now, null, "Automatic preassignment", true);
            await assignmentCommand.CreateAsync(assignment, cancellationToken);
            await statusHistoryCommand.AddAsync(new ReservationStatusHistory(createdReservation.Id, null, createdReservation.Status, now, null, "Reservation created with order orchestration"), cancellationToken);
        }, cancellationToken);

        try
        {
            var order = await orderCreationGateway.CreateAsync(
                new CreateExternalServiceOrderRequest(
                    createdReservation.Id,
                    request.ClientId,
                    request.ProviderEntityId,
                    assignedTechnicianId,
                    startAtUtc,
                    endAtUtc,
                    request.Items.Select(x => new CreateExternalServiceOrderItemRequest(x.ServiceId, x.ServiceName.Trim(), x.UnitPrice, x.Quantity)).ToArray()),
                cancellationToken);

            await eventPublisher.PublishAsync(new ReservationCreatedEvent(
                createdReservation.Id,
                createdReservation.ClientId,
                createdReservation.ProviderEntityId,
                createdReservation.ServiceId,
                assignedTechnicianId,
                createdReservation.StartAtUtc,
                createdReservation.EndAtUtc,
                now), cancellationToken);

            logger.LogInformation(
                "Reservation {ReservationId} and service order {OrderId} created in orchestrated flow.",
                createdReservation.Id,
                order.OrderId);

            return new ReservationOrderResponse(
                order.OrderId,
                createdReservation.Id,
                createdReservation.ClientId,
                createdReservation.ProviderEntityId,
                assignedTechnicianId,
                createdReservation.StartAtUtc,
                createdReservation.EndAtUtc,
                createdReservation.Status.ToString(),
                order.Status,
                order.TotalAmount);
        }
        catch
        {
            await reservationCommand.DeleteWithDependenciesAsync(createdReservation.Id, cancellationToken);
            throw;
        }
    }
}
