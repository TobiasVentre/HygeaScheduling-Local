using Microsoft.AspNetCore.Mvc;
using SchedulingMS.Application.DTOs;
using SchedulingMS.Application.Interfaces;

namespace SchedulingMS.Controllers;

[ApiController]
[Route("api/v1/availability")]
public class AvailabilityController(IAvailabilityService availabilityService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<AvailabilityResponse>> Create([FromBody] CreateAvailabilityRequest request, CancellationToken cancellationToken)
    {
        var result = await availabilityService.CreateAvailabilityAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetByTechnicianAndRange), new { technicianId = result.TechnicianId, from = result.StartAtUtc, to = result.EndAtUtc }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<AvailabilityResponse>> Update(Guid id, [FromBody] UpdateAvailabilityRequest request, CancellationToken cancellationToken)
    {
        var result = await availabilityService.UpdateAvailabilityAsync(id, request, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await availabilityService.DeleteAvailabilityAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpGet("technician/{technicianId:int}")]
    public async Task<ActionResult<IReadOnlyCollection<AvailabilityResponse>>> GetByTechnicianAndRange(
        int technicianId,
        [FromQuery] DateTime from,
        [FromQuery] DateTime to,
        CancellationToken cancellationToken)
    {
        var result = await availabilityService.GetAvailabilityByTechnicianAndRangeAsync(technicianId, from, to, cancellationToken);
        return Ok(result);
    }
}
