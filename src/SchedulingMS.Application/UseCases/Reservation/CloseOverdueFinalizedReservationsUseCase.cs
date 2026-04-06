using Microsoft.Extensions.Logging;
using SchedulingMS.Application.Events;
using SchedulingMS.Application.Interfaces.Commands;
using SchedulingMS.Application.Interfaces.Ports;
using SchedulingMS.Application.Interfaces.Queries;
using SchedulingMS.Application.Interfaces.UseCases.Reservation;
using SchedulingMS.Domain.Entities;
using SchedulingMS.Domain.Enums;

namespace SchedulingMS.Application.UseCases.Reservation;

public class CloseOverdueFinalizedReservationsUseCase(
    IReservationQuery reservationQuery,
    IReservationCommand reservationCommand,
    IReservationStatusHistoryCommand reservationStatusHistoryCommand,
    IUnitOfWork unitOfWork,
    IEventPublisher eventPublisher,
    ILogger<CloseOverdueFinalizedReservationsUseCase> logger) : ICloseOverdueFinalizedReservationsUseCase
{
    private const int DefaultRetentionHours = 24;
    private const int DefaultBatchSize = 100;

    public async Task<int> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        var thresholdUtc = DateTime.UtcNow.AddHours(-DefaultRetentionHours);
        var reservations = await reservationQuery.GetFinalizedPendingClosureAsync(thresholdUtc, DefaultBatchSize, cancellationToken);
        var closedCount = 0;

        foreach (var reservation in reservations)
        {
            if (reservation.Status != ReservationStatus.Finalized)
            {
                continue;
            }

            var previous = reservation.Status;
            var now = DateTime.UtcNow;

            await unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                reservation.ChangeStatus(ReservationStatus.Closed);
                await reservationCommand.UpdateStatusAsync(reservation.Id, reservation.Status, cancellationToken);
                await reservationStatusHistoryCommand.AddAsync(
                    new ReservationStatusHistory(
                        reservation.Id,
                        previous,
                        reservation.Status,
                        now,
                        null,
                        "Reservation closed automatically after 24 hours in finalized state."),
                    cancellationToken);
            }, cancellationToken);

            await eventPublisher.PublishAsync(new ReservationStatusChangedEvent(reservation.Id, previous, reservation.Status, now), cancellationToken);
            closedCount++;
        }

        if (closedCount > 0)
        {
            logger.LogInformation("Automatically closed {ClosedCount} finalized reservations.", closedCount);
        }

        return closedCount;
    }
}
