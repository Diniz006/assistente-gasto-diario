using AssistenteGastoDiario.Application.DTOs.Dashboard;
using AssistenteGastoDiario.Application.DTOs.FinancialCycles;
using AssistenteGastoDiario.Application.Interfaces;
using AssistenteGastoDiario.Domain.Enums;
using AssistenteGastoDiario.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AssistenteGastoDiario.Infrastructure.Services;

public sealed class DashboardQueryService : IDashboardQueryService
{
    private readonly AppDbContext _dbContext;
    private readonly IFinancialCycleService _financialCycleService;
    private readonly IDashboardService _dashboardService;

    public DashboardQueryService(
        AppDbContext dbContext,
        IFinancialCycleService financialCycleService,
        IDashboardService dashboardService)
    {
        _dbContext = dbContext;
        _financialCycleService = financialCycleService;
        _dashboardService = dashboardService;
    }

    public async Task<DashboardSummaryResult?> GetCurrentAsync(
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

        var incomeTotal = await _dbContext.Incomes
            .IgnoreQueryFilters()
            .Where(income =>
                income.UserId == userId
                && income.ReceivedOn >= cycle.CycleStartDate
                && income.ReceivedOn <= cycle.CycleEndDate)
            .Select(income => (decimal?)income.Amount)
            .SumAsync(cancellationToken) ?? 0m;

        if (incomeTotal <= 0m && setting.MonthlyIncomeDefault > 0m)
        {
            incomeTotal = setting.MonthlyIncomeDefault;
        }

        var fixedBillsTotal = await _dbContext.FixedBills
            .IgnoreQueryFilters()
            .Where(bill =>
                bill.UserId == userId
                && bill.AutoIncludeInCycle
                && bill.IsRecurringMonthly)
            .Select(bill => (decimal?)bill.Amount)
            .SumAsync(cancellationToken) ?? 0m;

        var expensesTotal = await _dbContext.Expenses
            .IgnoreQueryFilters()
            .Where(expense =>
                expense.UserId == userId
                && expense.SpentOn >= cycle.CycleStartDate
                && expense.SpentOn <= cycle.CycleEndDate)
            .Select(expense => (decimal?)expense.Amount)
            .SumAsync(cancellationToken) ?? 0m;

        var goalPlannedTotal = await _dbContext.FinancialGoals
            .IgnoreQueryFilters()
            .Where(goal =>
                goal.UserId == userId
                && goal.Status == GoalStatus.Active)
            .Select(goal => (decimal?)goal.MonthlyPlannedAmount)
            .SumAsync(cancellationToken) ?? 0m;

        var goalContributedTotal = await _dbContext.GoalContributions
            .IgnoreQueryFilters()
            .Where(contribution =>
                contribution.UserId == userId
                && contribution.ContributedOn >= cycle.CycleStartDate
                && contribution.ContributedOn <= cycle.CycleEndDate)
            .Select(contribution => (decimal?)contribution.Amount)
            .SumAsync(cancellationToken) ?? 0m;

        var openAlertsCount = await _dbContext.Alerts
            .IgnoreQueryFilters()
            .CountAsync(alert => alert.UserId == userId && !alert.IsRead, cancellationToken);

        return _dashboardService.Build(new DashboardSummaryRequest(
            referenceDate,
            setting.CycleStartDay,
            setting.MonthClosureDay,
            incomeTotal,
            fixedBillsTotal,
            goalPlannedTotal,
            expensesTotal,
            goalContributedTotal,
            openAlertsCount));
    }
}
