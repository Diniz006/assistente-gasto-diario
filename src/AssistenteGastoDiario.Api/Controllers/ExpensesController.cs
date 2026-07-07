using AssistenteGastoDiario.Api.Filters;
using AssistenteGastoDiario.Application.DTOs.Expenses;
using AssistenteGastoDiario.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssistenteGastoDiario.Api.Controllers;

[ApiController]
[Authorize]
[ServiceFilter(typeof(RequireMatchingUserIdFilter))]
[Route("api/users/{userId:guid}/expenses")]
public sealed class ExpensesController : ControllerBase
{
    private readonly IExpenseService _expenseService;

    public ExpensesController(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyCollection<ExpenseResponse>>> ListByPeriod(
        Guid userId,
        [FromQuery] DateOnly startDate,
        [FromQuery] DateOnly endDate,
        CancellationToken cancellationToken)
    {
        if (startDate > endDate)
        {
            return BadRequest(new { message = "startDate must be before or equal to endDate." });
        }

        var expenses = await _expenseService.ListByPeriodAsync(userId, startDate, endDate, cancellationToken);
        return Ok(expenses);
    }

    [HttpGet("{expenseId:guid}")]
    public async Task<ActionResult<ExpenseResponse>> GetById(Guid userId, Guid expenseId, CancellationToken cancellationToken)
    {
        var expense = await _expenseService.GetByIdAsync(userId, expenseId, cancellationToken);
        return expense is null ? NotFound() : Ok(expense);
    }

    [HttpPost]
    public async Task<ActionResult<ExpenseResponse>> Create(Guid userId, CreateExpenseRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var expense = await _expenseService.CreateAsync(userId, request, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { userId, expenseId = expense.Id }, expense);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    [HttpPut("{expenseId:guid}")]
    public async Task<ActionResult<ExpenseResponse>> Update(Guid userId, Guid expenseId, UpdateExpenseRequest request, CancellationToken cancellationToken)
    {
        var expense = await _expenseService.UpdateAsync(userId, expenseId, request, cancellationToken);
        return expense is null ? NotFound() : Ok(expense);
    }

    [HttpDelete("{expenseId:guid}")]
    public async Task<IActionResult> Delete(Guid userId, Guid expenseId, CancellationToken cancellationToken)
    {
        var deleted = await _expenseService.DeleteAsync(userId, expenseId, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
