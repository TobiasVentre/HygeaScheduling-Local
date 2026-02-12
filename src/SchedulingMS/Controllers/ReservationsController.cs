using Microsoft.AspNetCore.Mvc;
using SchedulingMS.Application.DTOs;
using SchedulingMS.Application.Interfaces;

namespace SchedulingMS.Controllers;

[ApiController]
[Route("api/v1/reservations")]
public class ReservationsController(IReservationService reservationService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ReservationResponse>> Create([FromBody] CreateReservationRequest request, CancellationToken cancellationToken)
    {
        var result = await reservationService.CreateReservationAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { reservationId = result.Id }, result);
    }

    [HttpGet("{reservationId:guid}")]
    public async Task<ActionResult<ReservationResponse>> GetById(Guid reservationId, CancellationToken cancellationToken)
    {
        var result = await reservationService.GetByIdAsync(reservationId, cancellationToken);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet("client/{clientId:int}")]
    public async Task<ActionResult<IReadOnlyCollection<ReservationResponse>>> GetByClient(int clientId, CancellationToken cancellationToken)
    {
        var result = await reservationService.GetByClientIdAsync(clientId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("technician/{technicianId:int}")]
    public async Task<ActionResult<IReadOnlyCollection<ReservationResponse>>> GetByTechnician(int technicianId, CancellationToken cancellationToken)
    {
        var result = await reservationService.GetByTechnicianIdAsync(technicianId, cancellationToken);
        return Ok(result);
    }

    [HttpPatch("{reservationId:guid}/status")]
    public async Task<ActionResult<ReservationResponse>> UpdateStatus(Guid reservationId, [FromBody] UpdateReservationStatusRequest request, CancellationToken cancellationToken)
    {
        var result = await reservationService.UpdateStatusAsync(reservationId, request, cancellationToken);
        return Ok(result);
    }
}
