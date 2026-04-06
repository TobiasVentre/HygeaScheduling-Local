using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SchedulingMS.Api.Security;
using SchedulingMS.Application.DTOs.Reservation;
using SchedulingMS.Application.Interfaces.UseCases.Reservation;

namespace SchedulingMS.Api.Controllers;

[ApiController]
[Route("api/v1/reservations")]
[Authorize]
public class ReservationsController(
    ICreateReservationUseCase createReservationUseCase,
    ICreateReservationWithOrderUseCase createReservationWithOrderUseCase,
    IGetReservationByIdUseCase getReservationByIdUseCase,
    IGetReservationsByClientUseCase getReservationsByClientUseCase,
    IGetReservationsByTechnicianUseCase getReservationsByTechnicianUseCase,
    IGetReservationsOverviewUseCase getReservationsOverviewUseCase,
    IApproveReservationUseCase approveReservationUseCase,
    IConfirmReservationUseCase confirmReservationUseCase,
    IUpdateReservationStatusUseCase updateReservationStatusUseCase,
    IReassignReservationUseCase reassignReservationUseCase) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = SecurityConstants.Policies.ClientOnly)]
    public async Task<ActionResult<ReservationResponse>> Create([FromBody] CreateReservationRequest request, CancellationToken cancellationToken)
    {
        var result = await createReservationUseCase.ExecuteAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { reservationId = result.Id }, result);
    }

    [HttpPost("with-order")]
    [Authorize(Policy = SecurityConstants.Policies.ClientOnly)]
    public async Task<ActionResult<ReservationOrderResponse>> CreateWithOrder([FromBody] CreateReservationWithOrderRequest request, CancellationToken cancellationToken)
    {
        var result = await createReservationWithOrderUseCase.ExecuteAsync(request, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{reservationId:guid}")]
    [Authorize(Policy = SecurityConstants.Policies.ProviderAdminOrAdmin)]
    public async Task<ActionResult<ReservationResponse>> GetById(Guid reservationId, CancellationToken cancellationToken)
    {
        var result = await getReservationByIdUseCase.ExecuteAsync(reservationId, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("client/{clientId:guid}")]
    [Authorize(Policy = SecurityConstants.Policies.ClientOnly)]
    public async Task<ActionResult<IReadOnlyCollection<ReservationResponse>>> GetByClient(Guid clientId, CancellationToken cancellationToken)
    {
        var result = await getReservationsByClientUseCase.ExecuteAsync(clientId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("technician/{technicianId:guid}")]
    [Authorize(Policy = SecurityConstants.Policies.TechnicianOnly)]
    public async Task<ActionResult<IReadOnlyCollection<ReservationResponse>>> GetByTechnician(Guid technicianId, CancellationToken cancellationToken)
    {
        var result = await getReservationsByTechnicianUseCase.ExecuteAsync(technicianId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("overview")]
    [Authorize(Policy = SecurityConstants.Policies.ProviderAdminOrAdmin)]
    public async Task<ActionResult<ReservationOverviewResponse>> GetOverview(
        [FromQuery] Guid? clientId,
        [FromQuery] Guid? technicianId,
        [FromQuery] Guid? providerEntityId,
        [FromQuery] Domain.Enums.ReservationStatus? status,
        [FromQuery] DateTime? fromUtc,
        [FromQuery] DateTime? toUtc,
        CancellationToken cancellationToken)
    {
        var result = await getReservationsOverviewUseCase.ExecuteAsync(
            new ReservationOverviewFilter(clientId, technicianId, providerEntityId, status, fromUtc, toUtc),
            cancellationToken);
        return Ok(result);
    }

    [HttpPatch("{reservationId:guid}/status")]
    [Authorize(Policy = SecurityConstants.Policies.ProviderAdminOrAdmin)]
    public async Task<ActionResult<ReservationResponse>> UpdateStatus(Guid reservationId, [FromBody] UpdateReservationStatusRequest request, CancellationToken cancellationToken)
    {
        var result = await updateReservationStatusUseCase.ExecuteAsync(reservationId, request, cancellationToken);
        return Ok(result);
    }

    [HttpPatch("{reservationId:guid}/approve")]
    [Authorize(Policy = SecurityConstants.Policies.ProviderAdminOrAdmin)]
    public async Task<ActionResult<ReservationResponse>> Approve(Guid reservationId, [FromBody] ApprovalDecisionRequest request, CancellationToken cancellationToken)
    {
        var result = await approveReservationUseCase.ExecuteAsync(reservationId, GetCurrentUserId(), request.Note, cancellationToken);
        return Ok(result);
    }

    [HttpPatch("{reservationId:guid}/confirm")]
    [Authorize(Policy = SecurityConstants.Policies.ProviderAdminOrAdmin)]
    public async Task<ActionResult<ReservationResponse>> Confirm(Guid reservationId, [FromBody] ApprovalDecisionRequest request, CancellationToken cancellationToken)
    {
        var result = await confirmReservationUseCase.ExecuteAsync(reservationId, GetCurrentUserId(), request.Note, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{reservationId:guid}/reassign")]
    [Authorize(Policy = SecurityConstants.Policies.ProviderAdminOrAdmin)]
    public async Task<ActionResult<ReservationResponse>> Reassign(Guid reservationId, [FromBody] ReassignReservationRequest request, CancellationToken cancellationToken)
    {
        if (request.OverrideByAdmin && !User.IsInRole(SecurityConstants.Roles.Admin))
        {
            return Forbid();
        }

        var result = await reassignReservationUseCase.ExecuteAsync(reservationId, request, cancellationToken);
        return Ok(result);
    }

    private Guid? GetCurrentUserId()
    {
        var claimValue = User.FindFirstValue("UserId")
                         ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(claimValue, out var userId) ? userId : null;
    }
}


