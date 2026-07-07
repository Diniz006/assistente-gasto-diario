using System.Security.Claims;
using AssistenteGastoDiario.Application.DTOs.CycleSummaries;
using AssistenteGastoDiario.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssistenteGastoDiario.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/me/cycle-summary")]
public sealed class MeCycleSummaryController : ControllerBase
{
    private readonly ICycleSummaryQueryService _cycleSummaryQueryService;

    public MeCycleSummaryController(ICycleSummaryQueryService cycleSummaryQueryService)
    {
        _cycleSummaryQueryService = cycleSummaryQueryService;
    }

    [HttpGet]
    public async Task<ActionResult<CycleSummaryResponse>> GetCurrent(
        [FromQuery] DateOnly? referenceDate,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var summary = await _cycleSummaryQueryService.GetCurrentAsync(
            userId.Value,
            referenceDate ?? DateOnly.FromDateTime(DateTime.Today),
            cancellationToken);

        return summary is null
            ? NotFound(new { message = "Financial settings not found for this user." })
            : Ok(summary);
    }

    private Guid? GetCurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out var userId) ? userId : null;
    }
}
