using SchedulingMS.Application.DTOs;
using SchedulingMS.Application.Events;
using SchedulingMS.Application.Exceptions;
using SchedulingMS.Application.Interfaces;
using SchedulingMS.Domain.Entities;
using SchedulingMS.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace SchedulingMS.Application.UseCases;

public class ReservationService(
    IReservationRepository reservationRepository,
    IAvailabilityRepository availabilityRepository,
    ITechnicianDirectoryGateway technicianDirectoryGateway,
    IEventPublisher eventPublisher,
    ILogger<ReservationService> logger) : IReservationService
{
    public async Task<ReservationResponse> CreateReservationAsync(CreateReservationRequest request, CancellationToken cancellationToken = default)
    {
        ValidateCreateRequest(request);

        var isApproved = await technicianDirectoryGateway.IsTechnicianApprovedAsync(request.TechnicianId, cancellationToken);
        if (!isApproved)
        {
            logger.LogWarning("Rejected reservation creation. Technician {TechnicianId} is not approved.", request.TechnicianId);
            throw new ValidationException("Technician is not approved.");
        }

        var availableSlots = await availabilityRepository.GetByTechnicianAndRangeAsync(request.TechnicianId, request.StartAtUtc, request.EndAtUtc, cancellationToken);
        var hasCoverage = availableSlots.Any(x => x.StartAtUtc <= request.StartAtUtc && x.EndAtUtc >= request.EndAtUtc);
        if (!hasCoverage)
        {
            logger.LogWarning("Rejected reservation creation due to missing availability. Technician {TechnicianId}, Start {StartAt}, End {EndAt}", request.TechnicianId, request.StartAtUtc, request.EndAtUtc);
            throw new ValidationException("No availability found for requested time range.");
        }

        var hasOverlap = await reservationRepository.ExistsOverlappingReservationAsync(request.TechnicianId, request.StartAtUtc, request.EndAtUtc, cancellationToken);
        if (hasOverlap)
        {
            logger.LogWarning("Rejected reservation creation due to overlap. Technician {TechnicianId}, Start {StartAt}, End {EndAt}", request.TechnicianId, request.StartAtUtc, request.EndAtUtc);
            throw new ConflictException("The technician already has a reservation in the requested time range.");
        }

        var reservation = new Reservation(request.ClientId, request.TechnicianId, request.ServiceId, request.StartAtUtc, request.EndAtUtc);
        var created = await reservationRepository.AddAsync(reservation, cancellationToken);

        logger.LogInformation("Reservation {ReservationId} created for Technician {TechnicianId} and Client {ClientId}.", created.Id, created.TechnicianId, created.ClientId);

        await eventPublisher.PublishAsync(new ReservationCreatedEvent(
            created.Id,
            created.ClientId,
            created.TechnicianId,
            created.ServiceId,
            created.StartAtUtc,
            created.Status,
            DateTime.UtcNow), cancellationToken);

        return Map(created);
    }

    public async Task<ReservationResponse?> GetByIdAsync(Guid reservationId, CancellationToken cancellationToken = default)
    {
        var reservation = await reservationRepository.GetByIdAsync(reservationId, cancellationToken);
        return reservation is null ? null : Map(reservation);
    }

    public async Task<IReadOnlyCollection<ReservationResponse>> GetByClientIdAsync(int clientId, CancellationToken cancellationToken = default)
    {
        if (clientId <= 0) throw new ValidationException("ClientId must be greater than 0.");
        var reservations = await reservationRepository.GetByClientIdAsync(clientId, cancellationToken);
        return reservations.Select(Map).ToArray();
    }

    public async Task<IReadOnlyCollection<ReservationResponse>> GetByTechnicianIdAsync(int technicianId, CancellationToken cancellationToken = default)
    {
        if (technicianId <= 0) throw new ValidationException("TechnicianId must be greater than 0.");
        var reservations = await reservationRepository.GetByTechnicianIdAsync(technicianId, cancellationToken);
        return reservations.Select(Map).ToArray();
    }

    public async Task<ReservationResponse> UpdateStatusAsync(Guid reservationId, UpdateReservationStatusRequest request, CancellationToken cancellationToken = default)
    {
        var reservation = await reservationRepository.GetByIdAsync(reservationId, cancellationToken)
                          ?? throw new NotFoundException($"Reservation {reservationId} was not found.");

        reservation.UpdateStatus(request.Status);
        await reservationRepository.UpdateStatusAsync(reservationId, request.Status, cancellationToken);

        logger.LogInformation("Reservation {ReservationId} changed status to {Status}.", reservationId, request.Status);

        if (request.Status == ReservationStatus.Finalized)
        {
            await eventPublisher.PublishAsync(new ReservationFinalizedEvent(
                reservation.Id,
                reservation.ClientId,
                reservation.TechnicianId,
                reservation.ServiceId,
                reservation.StartAtUtc,
                reservation.Status,
                DateTime.UtcNow), cancellationToken);
        }

        return Map(reservation);
    }

    private static ReservationResponse Map(Reservation reservation)
        => new(
            reservation.Id,
            reservation.ClientId,
            reservation.TechnicianId,
            reservation.ServiceId,
            reservation.StartAtUtc,
            reservation.EndAtUtc,
            reservation.Status,
            reservation.CreatedAtUtc);

    private static void ValidateCreateRequest(CreateReservationRequest request)
    {
        if (request.ClientId <= 0) throw new ValidationException("ClientId must be greater than 0.");
        if (request.TechnicianId <= 0) throw new ValidationException("TechnicianId must be greater than 0.");
        if (request.ServiceId == Guid.Empty) throw new ValidationException("ServiceId is required.");
        if (request.StartAtUtc >= request.EndAtUtc) throw new ValidationException("StartAtUtc must be before EndAtUtc.");
    }
}
