using AssistenteGastoDiario.Api.Filters;
using AssistenteGastoDiario.Application.DTOs.Dashboard;
using AssistenteGastoDiario.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssistenteGastoDiario.Api.Controllers;

[ApiController]
[Authorize]
[ServiceFilter(typeof(RequireMatchingUserIdFilter))]
[Route("api/users/{userId:guid}/dashboard")]
public sealed class DashboardController : ControllerBase
{
    private readonly IDashboardQueryService _dashboardQueryService;

    public DashboardController(IDashboardQueryService dashboardQueryService)
    {
        _dashboardQueryService = dashboardQueryService;
    }

    [HttpGet]
    public async Task<ActionResult<DashboardSummaryResult>> GetCurrent(
        Guid userId,
        [FromQuery] DateOnly? referenceDate,
        CancellationToken cancellationToken)
    {
        var dashboard = await _dashboardQueryService.GetCurrentAsync(
            userId,
            referenceDate ?? DateOnly.FromDateTime(DateTime.Today),
            cancellationToken);

        return dashboard is null
            ? NotFound(new { message = "Financial settings not found for this user." })
            : Ok(dashboard);
    }
}
