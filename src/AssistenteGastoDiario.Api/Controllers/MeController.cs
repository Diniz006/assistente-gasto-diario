using System.Security.Claims;
using AssistenteGastoDiario.Application.DTOs.Categories;
using AssistenteGastoDiario.Application.DTOs.Dashboard;
using AssistenteGastoDiario.Application.DTOs.Expenses;
using AssistenteGastoDiario.Application.DTOs.FinancialGoals;
using AssistenteGastoDiario.Application.DTOs.FinancialSettings;
using AssistenteGastoDiario.Application.DTOs.FixedBills;
using AssistenteGastoDiario.Application.DTOs.Incomes;
using AssistenteGastoDiario.Application.DTOs.Users;
using AssistenteGastoDiario.Application.Interfaces;
using AssistenteGastoDiario.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AssistenteGastoDiario.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/me")]
public sealed class MeController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IDashboardQueryService _dashboardQueryService;
    private readonly IFinancialSettingService _financialSettingService;
    private readonly ICategoryService _categoryService;
    private readonly IIncomeService _incomeService;
    private readonly IFixedBillService _fixedBillService;
    private readonly IExpenseService _expenseService;
    private readonly IFinancialGoalService _financialGoalService;

    public MeController(
        IUserService userService,
        IDashboardQueryService dashboardQueryService,
        IFinancialSettingService financialSettingService,
        ICategoryService categoryService,
        IIncomeService incomeService,
        IFixedBillService fixedBillService,
        IExpenseService expenseService,
        IFinancialGoalService financialGoalService)
    {
        _userService = userService;
        _dashboardQueryService = dashboardQueryService;
        _financialSettingService = financialSettingService;
        _categoryService = categoryService;
        _incomeService = incomeService;
        _fixedBillService = fixedBillService;
        _expenseService = expenseService;
        _financialGoalService = financialGoalService;
    }

    [HttpGet]
    public async Task<ActionResult<UserResponse>> GetProfile(CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var user = await _userService.GetByIdAsync(userId.Value, cancellationToken);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpGet("dashboard")]
    public async Task<ActionResult<DashboardSummaryResult>> GetDashboard(
        [FromQuery] DateOnly? referenceDate,
        CancellationToken cancellationToken)
    {
        var userId = GetCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var dashboard = await _dashboardQueryService.GetCurrentAsync(
            userId.Value,
            referenceDate ?? DateOnly.FromDateTime(DateTime.Today),
            cancellationToken);

        return dashboard is null
            ? NotFound(new { message = "Financial settings not found for this user." })
            : Ok(dashboard);
    }

    [HttpGet("financial-settings")]
    public async Task<ActionResult<FinancialSettingResponse>> GetFinancialSettings(CancellationToken cancellationToken)
    {
        var userId = GetRequiredCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var setting = await _financialSettingService.GetByUserIdAsync(userId.Value, cancellationToken);
        return setting is null ? NotFound() : Ok(setting);
    }

    [HttpPut("financial-settings")]
    public async Task<ActionResult<FinancialSettingResponse>> UpsertFinancialSettings(
        UpsertFinancialSettingRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetRequiredCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var setting = await _financialSettingService.UpsertAsync(userId.Value, request, cancellationToken);
        return Ok(setting);
    }

    [HttpGet("categories")]
    public async Task<ActionResult<IReadOnlyCollection<CategoryResponse>>> ListCategories(
        [FromQuery] CategoryType? type,
        CancellationToken cancellationToken)
    {
        var userId = GetRequiredCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var categories = await _categoryService.ListAsync(userId.Value, type, cancellationToken);
        return Ok(categories);
    }

    [HttpGet("categories/{categoryId:guid}")]
    public async Task<ActionResult<CategoryResponse>> GetCategory(
        Guid categoryId,
        CancellationToken cancellationToken)
    {
        var userId = GetRequiredCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var category = await _categoryService.GetByIdAsync(userId.Value, categoryId, cancellationToken);
        return category is null ? NotFound() : Ok(category);
    }

    [HttpPost("categories")]
    public async Task<ActionResult<CategoryResponse>> CreateCategory(
        CreateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetRequiredCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        try
        {
            var category = await _categoryService.CreateAsync(userId.Value, request, cancellationToken);
            return CreatedAtAction(nameof(GetCategory), new { categoryId = category.Id }, category);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpPut("categories/{categoryId:guid}")]
    public async Task<ActionResult<CategoryResponse>> UpdateCategory(
        Guid categoryId,
        UpdateCategoryRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetRequiredCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        try
        {
            var category = await _categoryService.UpdateAsync(userId.Value, categoryId, request, cancellationToken);
            return category is null ? NotFound() : Ok(category);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("categories/{categoryId:guid}")]
    public async Task<IActionResult> DeactivateCategory(Guid categoryId, CancellationToken cancellationToken)
    {
        var userId = GetRequiredCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var deactivated = await _categoryService.DeactivateAsync(userId.Value, categoryId, cancellationToken);
        return deactivated ? NoContent() : NotFound();
    }

    [HttpGet("incomes")]
    public async Task<ActionResult<IReadOnlyCollection<IncomeResponse>>> ListIncomes(
        [FromQuery] DateOnly startDate,
        [FromQuery] DateOnly endDate,
        CancellationToken cancellationToken)
    {
        var userId = GetRequiredCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        if (startDate > endDate)
        {
            return BadRequest(new { message = "startDate must be before or equal to endDate." });
        }

        var incomes = await _incomeService.ListByPeriodAsync(userId.Value, startDate, endDate, cancellationToken);
        return Ok(incomes);
    }

    [HttpGet("incomes/{incomeId:guid}")]
    public async Task<ActionResult<IncomeResponse>> GetIncome(Guid incomeId, CancellationToken cancellationToken)
    {
        var userId = GetRequiredCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var income = await _incomeService.GetByIdAsync(userId.Value, incomeId, cancellationToken);
        return income is null ? NotFound() : Ok(income);
    }

    [HttpPost("incomes")]
    public async Task<ActionResult<IncomeResponse>> CreateIncome(
        CreateIncomeRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetRequiredCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var income = await _incomeService.CreateAsync(userId.Value, request, cancellationToken);
        return CreatedAtAction(nameof(GetIncome), new { incomeId = income.Id }, income);
    }

    [HttpPut("incomes/{incomeId:guid}")]
    public async Task<ActionResult<IncomeResponse>> UpdateIncome(
        Guid incomeId,
        UpdateIncomeRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetRequiredCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var income = await _incomeService.UpdateAsync(userId.Value, incomeId, request, cancellationToken);
        return income is null ? NotFound() : Ok(income);
    }

    [HttpDelete("incomes/{incomeId:guid}")]
    public async Task<IActionResult> DeleteIncome(Guid incomeId, CancellationToken cancellationToken)
    {
        var userId = GetRequiredCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var deleted = await _incomeService.DeleteAsync(userId.Value, incomeId, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("fixed-bills")]
    public async Task<ActionResult<IReadOnlyCollection<FixedBillResponse>>> ListFixedBills(CancellationToken cancellationToken)
    {
        var userId = GetRequiredCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var fixedBills = await _fixedBillService.ListAsync(userId.Value, cancellationToken);
        return Ok(fixedBills);
    }

    [HttpGet("fixed-bills/{fixedBillId:guid}")]
    public async Task<ActionResult<FixedBillResponse>> GetFixedBill(
        Guid fixedBillId,
        CancellationToken cancellationToken)
    {
        var userId = GetRequiredCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var fixedBill = await _fixedBillService.GetByIdAsync(userId.Value, fixedBillId, cancellationToken);
        return fixedBill is null ? NotFound() : Ok(fixedBill);
    }

    [HttpPost("fixed-bills")]
    public async Task<ActionResult<FixedBillResponse>> CreateFixedBill(
        CreateFixedBillRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetRequiredCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var fixedBill = await _fixedBillService.CreateAsync(userId.Value, request, cancellationToken);
        return CreatedAtAction(nameof(GetFixedBill), new { fixedBillId = fixedBill.Id }, fixedBill);
    }

    [HttpPut("fixed-bills/{fixedBillId:guid}")]
    public async Task<ActionResult<FixedBillResponse>> UpdateFixedBill(
        Guid fixedBillId,
        UpdateFixedBillRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetRequiredCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var fixedBill = await _fixedBillService.UpdateAsync(userId.Value, fixedBillId, request, cancellationToken);
        return fixedBill is null ? NotFound() : Ok(fixedBill);
    }

    [HttpDelete("fixed-bills/{fixedBillId:guid}")]
    public async Task<IActionResult> DeleteFixedBill(Guid fixedBillId, CancellationToken cancellationToken)
    {
        var userId = GetRequiredCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var deleted = await _fixedBillService.DeleteAsync(userId.Value, fixedBillId, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("expenses")]
    public async Task<ActionResult<IReadOnlyCollection<ExpenseResponse>>> ListExpenses(
        [FromQuery] DateOnly startDate,
        [FromQuery] DateOnly endDate,
        CancellationToken cancellationToken)
    {
        var userId = GetRequiredCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        if (startDate > endDate)
        {
            return BadRequest(new { message = "startDate must be before or equal to endDate." });
        }

        var expenses = await _expenseService.ListByPeriodAsync(userId.Value, startDate, endDate, cancellationToken);
        return Ok(expenses);
    }

    [HttpGet("expenses/{expenseId:guid}")]
    public async Task<ActionResult<ExpenseResponse>> GetExpense(
        Guid expenseId,
        CancellationToken cancellationToken)
    {
        var userId = GetRequiredCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var expense = await _expenseService.GetByIdAsync(userId.Value, expenseId, cancellationToken);
        return expense is null ? NotFound() : Ok(expense);
    }

    [HttpPost("expenses")]
    public async Task<ActionResult<ExpenseResponse>> CreateExpense(
        CreateExpenseRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetRequiredCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var expense = await _expenseService.CreateAsync(userId.Value, request, cancellationToken);
        return CreatedAtAction(nameof(GetExpense), new { expenseId = expense.Id }, expense);
    }

    [HttpPut("expenses/{expenseId:guid}")]
    public async Task<ActionResult<ExpenseResponse>> UpdateExpense(
        Guid expenseId,
        UpdateExpenseRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetRequiredCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var expense = await _expenseService.UpdateAsync(userId.Value, expenseId, request, cancellationToken);
        return expense is null ? NotFound() : Ok(expense);
    }

    [HttpDelete("expenses/{expenseId:guid}")]
    public async Task<IActionResult> DeleteExpense(Guid expenseId, CancellationToken cancellationToken)
    {
        var userId = GetRequiredCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var deleted = await _expenseService.DeleteAsync(userId.Value, expenseId, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("financial-goals")]
    public async Task<ActionResult<IReadOnlyCollection<FinancialGoalResponse>>> ListFinancialGoals(CancellationToken cancellationToken)
    {
        var userId = GetRequiredCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var goals = await _financialGoalService.ListAsync(userId.Value, cancellationToken);
        return Ok(goals);
    }

    [HttpGet("financial-goals/{financialGoalId:guid}")]
    public async Task<ActionResult<FinancialGoalResponse>> GetFinancialGoal(
        Guid financialGoalId,
        CancellationToken cancellationToken)
    {
        var userId = GetRequiredCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var goal = await _financialGoalService.GetByIdAsync(userId.Value, financialGoalId, cancellationToken);
        return goal is null ? NotFound() : Ok(goal);
    }

    [HttpPost("financial-goals")]
    public async Task<ActionResult<FinancialGoalResponse>> CreateFinancialGoal(
        CreateFinancialGoalRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetRequiredCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var goal = await _financialGoalService.CreateAsync(userId.Value, request, cancellationToken);
        return CreatedAtAction(nameof(GetFinancialGoal), new { financialGoalId = goal.Id }, goal);
    }

    [HttpPut("financial-goals/{financialGoalId:guid}")]
    public async Task<ActionResult<FinancialGoalResponse>> UpdateFinancialGoal(
        Guid financialGoalId,
        UpdateFinancialGoalRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetRequiredCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var goal = await _financialGoalService.UpdateAsync(userId.Value, financialGoalId, request, cancellationToken);
        return goal is null ? NotFound() : Ok(goal);
    }

    [HttpDelete("financial-goals/{financialGoalId:guid}")]
    public async Task<IActionResult> DeleteFinancialGoal(Guid financialGoalId, CancellationToken cancellationToken)
    {
        var userId = GetRequiredCurrentUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var deleted = await _financialGoalService.DeleteAsync(userId.Value, financialGoalId, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }

    private Guid? GetCurrentUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out var userId) ? userId : null;
    }

    private Guid? GetRequiredCurrentUserId() => GetCurrentUserId();
}
