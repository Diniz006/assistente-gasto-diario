using System.Security.Claims;
using AssistenteGastoDiario.Application.DTOs.QuickExpenses;
using AssistenteGastoDiario.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssistenteGastoDiario.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/me/quick-expenses")]
public sealed class MeQuickExpensesController : ControllerBase
{
    private readonly IQuickExpenseService _quickExpenseService;

    public MeQuickExpensesController(IQuickExpenseService quickExpenseService)
    {
        _quickExpenseService = quickExpenseService;
    }

    [HttpPost]
    public async Task<ActionResult<QuickExpenseResponse>> Create(
        CreateQuickExpenseRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        try
        {
            var response = await _quickExpenseService.CreateAsync(userId.Value, request, cancellationToken);
            return CreatedAtAction(
                nameof(MeController.GetExpense),
                "Me",
                new { expenseId = response.Expense.Id },
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
