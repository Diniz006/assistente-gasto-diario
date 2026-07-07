using AssistenteGastoDiario.Application.DTOs.FinancialCycles;

namespace AssistenteGastoDiario.Application.DTOs.CycleSummaries;

public sealed record CycleSummaryResponse(
    FinancialCycleResult FinancialCycle,
    decimal IncomeTotal,
    decimal FixedBillsTotal,
    decimal ExpensesTotal,
    decimal GoalPlannedTotal,
    decimal GoalContributedTotal,
    decimal AvailableBalance,
    decimal SafeDailyLimit,
    int IncomeCount,
    int FixedBillCount,
    int ExpenseCount,
    int ActiveGoalCount,
    IReadOnlyCollection<ExpenseCategorySummaryItem> ExpensesByCategory,
    DateTimeOffset GeneratedAt);
