namespace AssistenteGastoDiario.Application.DTOs.SafeDailyLimits;

public sealed record SafeDailyLimitCalculationRequest(
    decimal IncomeTotal,
    decimal FixedBillsTotal,
    decimal GoalPlannedTotal,
    decimal ExpensesTotal,
    int DaysRemaining);
