using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchedulingMS.Api.Security;
using SchedulingMS.Application.DTOs.Absence;
using SchedulingMS.Application.Interfaces.UseCases.Absence;

namespace SchedulingMS.Api.Controllers;

[ApiController]
[Route("api/v1/absences")]
[Authorize]
public class AbsencesController(
    ICreateAbsenceUseCase createAbsenceUseCase,
    IUpdateAbsenceUseCase updateAbsenceUseCase,
    IDeleteAbsenceUseCase deleteAbsenceUseCase,
    IGetAbsencesByTechnicianUseCase getAbsencesByTechnicianUseCase) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = SecurityConstants.Policies.TechnicianOnly)]
    public async Task<ActionResult<AbsenceResponse>> Create([FromBody] CreateAbsenceRequest request, CancellationToken cancellationToken)
    {
        var result = await createAbsenceUseCase.ExecuteAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetByTechnicianAndRange), new { technicianId = result.TechnicianId, fromUtc = result.StartAtUtc, toUtc = result.EndAtUtc }, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = SecurityConstants.Policies.TechnicianOnly)]
    public async Task<ActionResult<AbsenceResponse>> Update(Guid id, [FromBody] UpdateAbsenceRequest request, CancellationToken cancellationToken)
    {
        var result = await updateAbsenceUseCase.ExecuteAsync(id, request, cancellationToken);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = SecurityConstants.Policies.TechnicianOnly)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await deleteAbsenceUseCase.ExecuteAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpGet("technician/{technicianId:guid}")]
    [Authorize(Policy = SecurityConstants.Policies.ClientOrTechnicianOrProviderAdminOrAdmin)]
    public async Task<ActionResult<IReadOnlyCollection<AbsenceResponse>>> GetByTechnicianAndRange(
        Guid technicianId,
        [FromQuery] DateTime fromUtc,
        [FromQuery] DateTime toUtc,
        CancellationToken cancellationToken)
    {
        var result = await getAbsencesByTechnicianUseCase.ExecuteAsync(technicianId, fromUtc, toUtc, cancellationToken);
        return Ok(result);
    }
}


