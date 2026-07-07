using AssistenteGastoDiario.Application.DTOs.CycleSummaries;
using AssistenteGastoDiario.Application.DTOs.FinancialCycles;
using AssistenteGastoDiario.Application.DTOs.SafeDailyLimits;
using AssistenteGastoDiario.Application.Interfaces;
using AssistenteGastoDiario.Domain.Enums;
using AssistenteGastoDiario.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AssistenteGastoDiario.Infrastructure.Services;

public sealed class CycleSummaryQueryService : ICycleSummaryQueryService
{
    private readonly AppDbContext _dbContext;
    private readonly IFinancialCycleService _financialCycleService;
    private readonly ISafeDailyLimitService _safeDailyLimitService;

    public CycleSummaryQueryService(
        AppDbContext dbContext,
        IFinancialCycleService financialCycleService,
        ISafeDailyLimitService safeDailyLimitService)
    {
        _dbContext = dbContext;
        _financialCycleService = financialCycleService;
        _safeDailyLimitService = safeDailyLimitService;
    }

    public async Task<CycleSummaryResponse?> GetCurrentAsync(
        Guid userId,
        DateOnly referenceDate,
        CancellationToken cancellationToken = default)
    {
        var setting = await _dbContext.FinancialSettings
            .IgnoreQueryFilters()
            .AsNoTracking()
            .FirstOrDefaultAsync(item => item.UserId == userId, cancellationToken);

        if (setting is null)
        {
            return null;
        }

        var cycle = _financialCycleService.Calculate(new FinancialCycleCalculationRequest(
            referenceDate,
            setting.CycleStartDay,
            setting.MonthClosureDay));

        var incomeQuery = _dbContext.Incomes
            .IgnoreQueryFilters()
            .Where(income =>
                income.UserId == userId
                && income.ReceivedOn >= cycle.CycleStartDate
                && income.ReceivedOn <= cycle.CycleEndDate);

        var fixedBillsQuery = _dbContext.FixedBills
            .IgnoreQueryFilters()
            .Where(bill =>
                bill.UserId == userId
                && bill.AutoIncludeInCycle
                && bill.IsRecurringMonthly);

        var expensesQuery = _dbContext.Expenses
            .IgnoreQueryFilters()
            .Where(expense =>
                expense.UserId == userId
                && expense.SpentOn >= cycle.CycleStartDate
                && expense.SpentOn <= cycle.CycleEndDate);

        var goalsQuery = _dbContext.FinancialGoals
            .IgnoreQueryFilters()
            .Where(goal =>
                goal.UserId == userId
                && goal.Status == GoalStatus.Active);

        var contributionsQuery = _dbContext.GoalContributions
            .IgnoreQueryFilters()
            .Where(contribution =>
                contribution.UserId == userId
                && contribution.ContributedOn >= cycle.CycleStartDate
                && contribution.ContributedOn <= cycle.CycleEndDate);

        var incomeTotal = await incomeQuery.Select(income => (decimal?)income.Amount).SumAsync(cancellationToken) ?? 0m;
        if (incomeTotal <= 0m && setting.MonthlyIncomeDefault > 0m)
        {
            incomeTotal = setting.MonthlyIncomeDefault;
        }

        var fixedBillsTotal = await fixedBillsQuery.Select(bill => (decimal?)bill.Amount).SumAsync(cancellationToken) ?? 0m;
        var expensesTotal = await expensesQuery.Select(expense => (decimal?)expense.Amount).SumAsync(cancellationToken) ?? 0m;
        var goalPlannedTotal = await goalsQuery.Select(goal => (decimal?)goal.MonthlyPlannedAmount).SumAsync(cancellationToken) ?? 0m;
        var goalContributedTotal = await contributionsQuery.Select(contribution => (decimal?)contribution.Amount).SumAsync(cancellationToken) ?? 0m;

        var safeDailyLimit = _safeDailyLimitService.Calculate(new SafeDailyLimitCalculationRequest(
            incomeTotal,
            fixedBillsTotal,
            goalPlannedTotal,
            expensesTotal,
            cycle.DaysRemaining));

        var expenseCategoryRows = await (
                from expense in expensesQuery
                join category in _dbContext.Categories.IgnoreQueryFilters().AsNoTracking()
                    on expense.CategoryId equals category.Id
                group expense by new { expense.CategoryId, category.Name } into categoryGroup
                select new
                {
                    categoryGroup.Key.CategoryId,
                    CategoryName = categoryGroup.Key.Name,
                    Amount = categoryGroup.Sum(expense => expense.Amount),
                    ExpenseCount = categoryGroup.Count()
                })
            .OrderByDescending(item => item.Amount)
            .ToListAsync(cancellationToken);

        var expensesByCategory = expenseCategoryRows
            .Select(item => new ExpenseCategorySummaryItem(
                item.CategoryId,
                item.CategoryName,
                item.Amount,
                item.ExpenseCount))
            .ToList();

        return new CycleSummaryResponse(
            cycle,
            incomeTotal,
            fixedBillsTotal,
            expensesTotal,
            goalPlannedTotal,
            goalContributedTotal,
            safeDailyLimit.AvailableBalance,
            safeDailyLimit.SafeDailyLimit,
            await incomeQuery.CountAsync(cancellationToken),
            await fixedBillsQuery.CountAsync(cancellationToken),
            await expensesQuery.CountAsync(cancellationToken),
            await goalsQuery.CountAsync(cancellationToken),
            expensesByCategory,
            DateTimeOffset.UtcNow);
    }
}
