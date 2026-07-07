using AssistenteGastoDiario.Application.DTOs.Dashboard;
using AssistenteGastoDiario.Application.DTOs.FinancialGoals;

namespace AssistenteGastoDiario.Application.DTOs.GoalContributions;

public sealed record GoalContributionCreatedResponse(
    GoalContributionResponse Contribution,
    FinancialGoalResponse Goal,
    DashboardSummaryResult? Dashboard);
