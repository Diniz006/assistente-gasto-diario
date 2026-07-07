using AssistenteGastoDiario.Api.Filters;
using AssistenteGastoDiario.Application.DTOs.Incomes;
using AssistenteGastoDiario.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssistenteGastoDiario.Api.Controllers;

[ApiController]
[Authorize]
[ServiceFilter(typeof(RequireMatchingUserIdFilter))]
[Route("api/users/{userId:guid}/incomes")]
public sealed class IncomesController : ControllerBase
{
    private readonly IIncomeService _incomeService;

    public IncomesController(IIncomeService incomeService)
    {
        _incomeService = incomeService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<IncomeResponse>>> ListByPeriod(
        Guid userId,
        [FromQuery] DateOnly startDate,
        [FromQuery] DateOnly endDate,
        CancellationToken cancellationToken)
    {
        if (startDate > endDate)
        {
            return BadRequest(new { message = "startDate must be before or equal to endDate." });
        }

        var incomes = await _incomeService.ListByPeriodAsync(userId, startDate, endDate, cancellationToken);
        return Ok(incomes);
    }

    [HttpGet("{incomeId:guid}")]
    public async Task<ActionResult<IncomeResponse>> GetById(Guid userId, Guid incomeId, CancellationToken cancellationToken)
    {
        var income = await _incomeService.GetByIdAsync(userId, incomeId, cancellationToken);
        return income is null ? NotFound() : Ok(income);
    }

    [HttpPost]
    public async Task<ActionResult<IncomeResponse>> Create(Guid userId, CreateIncomeRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var income = await _incomeService.CreateAsync(userId, request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { userId, incomeId = income.Id }, income);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPut("{incomeId:guid}")]
    public async Task<ActionResult<IncomeResponse>> Update(Guid userId, Guid incomeId, UpdateIncomeRequest request, CancellationToken cancellationToken)
    {
        var income = await _incomeService.UpdateAsync(userId, incomeId, request, cancellationToken);
        return income is null ? NotFound() : Ok(income);
    }

    [HttpDelete("{incomeId:guid}")]
    public async Task<IActionResult> Delete(Guid userId, Guid incomeId, CancellationToken cancellationToken)
    {
        var deleted = await _incomeService.DeleteAsync(userId, incomeId, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
