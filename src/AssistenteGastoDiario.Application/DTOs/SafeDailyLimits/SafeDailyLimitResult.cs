namespace AssistenteGastoDiario.Application.DTOs.SafeDailyLimits;

public sealed record SafeDailyLimitResult(
    decimal IncomeTotal,
    decimal FixedBillsTotal,
    decimal GoalPlannedTotal,
    decimal ExpensesTotal,
    decimal AvailableBalance,
    int DaysRemaining,
    decimal SafeDailyLimit,
    string Message);
