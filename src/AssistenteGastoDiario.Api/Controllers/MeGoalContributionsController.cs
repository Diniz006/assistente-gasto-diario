using System.Security.Claims;
using AssistenteGastoDiario.Application.DTOs.GoalContributions;
using AssistenteGastoDiario.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssistenteGastoDiario.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/me/financial-goals/{financialGoalId:guid}/contributions")]
public sealed class MeGoalContributionsController : ControllerBase
{
    private readonly IGoalContributionService _goalContributionService;

    public MeGoalContributionsController(IGoalContributionService goalContributionService)
    {
        _goalContributionService = goalContributionService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<GoalContributionResponse>>> ListByGoal(
        Guid financialGoalId,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        try
        {
            var contributions = await _goalContributionService.ListByGoalAsync(
                userId.Value,
                financialGoalId,
                cancellationToken);

            return Ok(contributions);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<GoalContributionCreatedResponse>> Create(
        Guid financialGoalId,
        CreateGoalContributionRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        try
        {
            var response = await _goalContributionService.CreateAsync(
                userId.Value,
                financialGoalId,
                request,
                cancellationToken);

            return CreatedAtAction(
                nameof(ListByGoal),
                new { financialGoalId },
                response);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private Guid? GetCurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out var userId) ? userId : null;
    }
}
