using AssistenteGastoDiario.Api.Filters;
using AssistenteGastoDiario.Application.DTOs.FinancialSettings;
using AssistenteGastoDiario.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssistenteGastoDiario.Api.Controllers;

[ApiController]
[Authorize]
[ServiceFilter(typeof(RequireMatchingUserIdFilter))]
[Route("api/users/{userId:guid}/financial-settings")]
public sealed class FinancialSettingsController : ControllerBase
{
    private readonly IFinancialSettingService _financialSettingService;

    public FinancialSettingsController(IFinancialSettingService financialSettingService)
    {
        _financialSettingService = financialSettingService;
    }

    [HttpGet]
    public async Task<ActionResult<FinancialSettingResponse>> Get(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var setting = await _financialSettingService.GetByUserIdAsync(userId, cancellationToken);
        return setting is null ? NotFound() : Ok(setting);
    }

    [HttpPut]
    public async Task<ActionResult<FinancialSettingResponse>> Upsert(
        Guid userId,
        UpsertFinancialSettingRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var setting = await _financialSettingService.UpsertAsync(userId, request, cancellationToken);
            return Ok(setting);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}
