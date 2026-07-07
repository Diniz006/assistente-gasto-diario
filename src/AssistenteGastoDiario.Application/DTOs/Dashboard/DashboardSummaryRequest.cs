namespace AssistenteGastoDiario.Application.DTOs.Dashboard;

public sealed record DashboardSummaryRequest(
    DateOnly ReferenceDate,
    int CycleStartDay,
    int? MonthClosureDay,
    decimal IncomeTotal,
    decimal FixedBillsTotal,
    decimal GoalPlannedTotal,
    decimal ExpensesTotal,
    decimal GoalContributedTotal,
    int OpenAlertsCount);
