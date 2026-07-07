using AssistenteGastoDiario.Application.DTOs.Expenses;
using AssistenteGastoDiario.Application.DTOs.QuickExpenses;
using AssistenteGastoDiario.Application.Interfaces;
using AssistenteGastoDiario.Domain.Enums;
using AssistenteGastoDiario.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AssistenteGastoDiario.Infrastructure.Services;

public sealed class QuickExpenseService : IQuickExpenseService
{
    private readonly AppDbContext _dbContext;
    private readonly IExpenseService _expenseService;
    private readonly IDashboardQueryService _dashboardQueryService;

    public QuickExpenseService(
        AppDbContext dbContext,
        IExpenseService expenseService,
        IDashboardQueryService dashboardQueryService)
    {
        _dbContext = dbContext;
        _expenseService = expenseService;
        _dashboardQueryService = dashboardQueryService;
    }

    public async Task<QuickExpenseResponse> CreateAsync(
        Guid userId,
        CreateQuickExpenseRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var spentOn = request.SpentOn ?? DateOnly.FromDateTime(DateTime.Today);
        var categoryId = request.CategoryId ?? await GetDefaultExpenseCategoryIdAsync(userId, cancellationToken);

        var expense = await _expenseService.CreateAsync(
            userId,
            new CreateExpenseRequest(
                categoryId,
                request.Description,
                request.Amount,
                spentOn,
                request.PaymentMethod,
                request.Notes),
            cancellationToken);

        var dashboard = await _dashboardQueryService.GetCurrentAsync(userId, spentOn, cancellationToken);

        return new QuickExpenseResponse(expense, dashboard);
    }

    private async Task<Guid> GetDefaultExpenseCategoryIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        var categoryId = await _dbContext.Categories
            .IgnoreQueryFilters()
            .AsNoTracking()
            .Where(category =>
                category.UserId == userId
                && category.Type == CategoryType.Expense
                && category.IsActive)
            .OrderByDescending(category => category.IsDefault)
            .ThenBy(category => category.Name)
            .Select(category => (Guid?)category.Id)
            .FirstOrDefaultAsync(cancellationToken);

        return categoryId ?? throw new InvalidOperationException("No active expense category found for this user.");
    }
}
