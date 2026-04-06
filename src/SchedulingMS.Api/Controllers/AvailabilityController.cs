using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchedulingMS.Api.Security;
using SchedulingMS.Application.DTOs.Availability;
using SchedulingMS.Application.Interfaces.UseCases.Availability;

namespace SchedulingMS.Api.Controllers;

[ApiController]
[Route("api/v1/availability")]
[Authorize]
public class AvailabilityController(
    ICreateAvailabilityUseCase createAvailabilityUseCase,
    IUpdateAvailabilityUseCase updateAvailabilityUseCase,
    IDeleteAvailabilityUseCase deleteAvailabilityUseCase,
    IGetAvailabilityByTechnicianUseCase getAvailabilityByTechnicianUseCase) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = SecurityConstants.Policies.TechnicianOnly)]
    public async Task<ActionResult<AvailabilityResponse>> Create([FromBody] CreateAvailabilityRequest request, CancellationToken cancellationToken)
    {
        var result = await createAvailabilityUseCase.ExecuteAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetByTechnicianAndRange), new { technicianId = result.TechnicianId, fromUtc = result.StartAtUtc, toUtc = result.EndAtUtc }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = SecurityConstants.Policies.TechnicianOnly)]
    public async Task<ActionResult<AvailabilityResponse>> Update(Guid id, [FromBody] UpdateAvailabilityRequest request, CancellationToken cancellationToken)
    {
        var result = await updateAvailabilityUseCase.ExecuteAsync(id, request, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = SecurityConstants.Policies.TechnicianOnly)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await deleteAvailabilityUseCase.ExecuteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpGet("technician/{technicianId:guid}")]
    [Authorize(Policy = SecurityConstants.Policies.TechnicianOnly)]
    public async Task<ActionResult<IReadOnlyCollection<AvailabilityResponse>>> GetByTechnicianAndRange(
        Guid technicianId,
        [FromQuery] DateTime fromUtc,
        [FromQuery] DateTime toUtc,
        CancellationToken cancellationToken)
    {
        var result = await getAvailabilityByTechnicianUseCase.ExecuteAsync(technicianId, fromUtc, toUtc, cancellationToken);
        return Ok(result);
    }
}


