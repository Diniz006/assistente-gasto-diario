using AssistenteGastoDiario.Application.DTOs.FinancialCycles;
using AssistenteGastoDiario.Application.DTOs.SafeDailyLimits;

namespace AssistenteGastoDiario.Application.DTOs.Dashboard;

public sealed record DashboardSummaryResult(
    FinancialCycleResult FinancialCycle,
    SafeDailyLimitResult SafeDailyLimit,
    decimal GoalContributedTotal,
    int OpenAlertsCount,
    IReadOnlyCollection<DashboardAlertItem> Alerts,
    DateTimeOffset GeneratedAt);
