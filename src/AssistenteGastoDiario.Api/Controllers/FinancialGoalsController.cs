using AssistenteGastoDiario.Api.Filters;
using AssistenteGastoDiario.Application.DTOs.FinancialGoals;
using AssistenteGastoDiario.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssistenteGastoDiario.Api.Controllers;

[ApiController]
[Authorize]
[ServiceFilter(typeof(RequireMatchingUserIdFilter))]
[Route("api/users/{userId:guid}/financial-goals")]
public sealed class FinancialGoalsController : ControllerBase
{
    private readonly IFinancialGoalService _financialGoalService;

    public FinancialGoalsController(IFinancialGoalService financialGoalService)
    {
        _financialGoalService = financialGoalService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<FinancialGoalResponse>>> List(Guid userId, CancellationToken cancellationToken)
    {
        var goals = await _financialGoalService.ListAsync(userId, cancellationToken);
        return Ok(goals);
    }

    [HttpGet("{financialGoalId:guid}")]
    public async Task<ActionResult<FinancialGoalResponse>> GetById(Guid userId, Guid financialGoalId, CancellationToken cancellationToken)
    {
        var goal = await _financialGoalService.GetByIdAsync(userId, financialGoalId, cancellationToken);
        return goal is null ? NotFound() : Ok(goal);
    }

    [HttpPost]
    public async Task<ActionResult<FinancialGoalResponse>> Create(Guid userId, CreateFinancialGoalRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var goal = await _financialGoalService.CreateAsync(userId, request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { userId, financialGoalId = goal.Id }, goal);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPut("{financialGoalId:guid}")]
    public async Task<ActionResult<FinancialGoalResponse>> Update(Guid userId, Guid financialGoalId, UpdateFinancialGoalRequest request, CancellationToken cancellationToken)
    {
        var goal = await _financialGoalService.UpdateAsync(userId, financialGoalId, request, cancellationToken);
        return goal is null ? NotFound() : Ok(goal);
    }

    [HttpDelete("{financialGoalId:guid}")]
    public async Task<IActionResult> Delete(Guid userId, Guid financialGoalId, CancellationToken cancellationToken)
    {
        var deleted = await _financialGoalService.DeleteAsync(userId, financialGoalId, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
