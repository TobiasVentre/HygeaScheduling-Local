using SchedulingMS.Application.DTOs.Reservation;
using SchedulingMS.Application.Events;
using SchedulingMS.Application.Exceptions;
using SchedulingMS.Application.Interfaces.Commands;
using SchedulingMS.Application.Interfaces.Ports;
using SchedulingMS.Application.Interfaces.Queries;
using SchedulingMS.Application.Interfaces.UseCases.Reservation;
using SchedulingMS.Domain.Entities;
using SchedulingMS.Domain.Enums;

namespace SchedulingMS.Application.UseCases.Reservation;

public class UpdateReservationStatusUseCase(
    IReservationQuery reservationQuery,
    IReservationCommand reservationCommand,
    IAssignmentQuery assignmentQuery,
    IReservationStatusHistoryCommand statusHistoryCommand,
    IUnitOfWork unitOfWork,
    IEventPublisher eventPublisher) : IUpdateReservationStatusUseCase
{
    public async Task<ReservationResponse> ExecuteAsync(Guid reservationId, UpdateReservationStatusRequest request, CancellationToken cancellationToken = default)
    {
        var reservation = await reservationQuery.GetByIdAsync(reservationId, cancellationToken)
            ?? throw new NotFoundException($"Reservation {reservationId} was not found.");

        EnsureTransitionAllowed(reservation.Status, request.Status);

        var previous = reservation.Status;
        var now = DateTime.UtcNow;

        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            reservation.ChangeStatus(request.Status);
            await reservationCommand.UpdateStatusAsync(reservation.Id, reservation.Status, cancellationToken);
            await statusHistoryCommand.AddAsync(new ReservationStatusHistory(reservation.Id, previous, reservation.Status, now, request.ChangedByUserId, request.Note), cancellationToken);
        }, cancellationToken);

        await eventPublisher.PublishAsync(new ReservationStatusChangedEvent(reservation.Id, previous, reservation.Status, now), cancellationToken);

        var currentAssignment = await assignmentQuery.GetCurrentByReservationIdAsync(reservation.Id, cancellationToken)
            ?? throw new NotFoundException($"Reservation {reservation.Id} has no current assignment.");

        return new ReservationResponse(
            reservation.Id,
            reservation.ClientId,
            reservation.ProviderEntityId,
            reservation.ServiceId,
            currentAssignment.TechnicianId,
            reservation.StartAtUtc,
            reservation.EndAtUtc,
            reservation.Status,
            reservation.CreatedAtUtc);
    }

    private static void EnsureTransitionAllowed(ReservationStatus current, ReservationStatus next)
    {
        var valid = (current, next) switch
        {
            (ReservationStatus.Created, ReservationStatus.Approved) => true,
            (ReservationStatus.Created, ReservationStatus.Exception) => true,
            (ReservationStatus.Approved, ReservationStatus.Confirmed) => true,
            (ReservationStatus.Approved, ReservationStatus.Exception) => true,
            (ReservationStatus.Confirmed, ReservationStatus.InProgress) => true,
            (ReservationStatus.Confirmed, ReservationStatus.Exception) => true,
            (ReservationStatus.InProgress, ReservationStatus.Finalized) => true,
            (ReservationStatus.InProgress, ReservationStatus.Exception) => true,
            (ReservationStatus.Finalized, ReservationStatus.Closed) => true,
            (ReservationStatus.Exception, ReservationStatus.Closed) => true,
            _ => false
        };

        if (!valid)
            throw new ValidationException($"Invalid reservation status transition from {current} to {next}.");
    }
}


