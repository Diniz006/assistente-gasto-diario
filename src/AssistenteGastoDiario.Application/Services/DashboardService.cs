using AssistenteGastoDiario.Application.DTOs.Dashboard;
using AssistenteGastoDiario.Application.DTOs.FinancialCycles;
using AssistenteGastoDiario.Application.DTOs.SafeDailyLimits;
using AssistenteGastoDiario.Application.Interfaces;
using AssistenteGastoDiario.Domain.Enums;

namespace AssistenteGastoDiario.Application.Services;

public sealed class DashboardService : IDashboardService
{
    private readonly IFinancialCycleService _financialCycleService;
    private readonly ISafeDailyLimitService _safeDailyLimitService;

    public DashboardService(
        IFinancialCycleService financialCycleService,
        ISafeDailyLimitService safeDailyLimitService)
    {
        _financialCycleService = financialCycleService;
        _safeDailyLimitService = safeDailyLimitService;
    }

    public DashboardSummaryResult Build(DashboardSummaryRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        var financialCycle = _financialCycleService.Calculate(new FinancialCycleCalculationRequest(
            request.ReferenceDate,
            request.CycleStartDay,
            request.MonthClosureDay));

        var safeDailyLimit = _safeDailyLimitService.Calculate(new SafeDailyLimitCalculationRequest(
            request.IncomeTotal,
            request.FixedBillsTotal,
            request.GoalPlannedTotal,
            request.ExpensesTotal,
            financialCycle.DaysRemaining));

        var alerts = BuildAlerts(safeDailyLimit, financialCycle, request.OpenAlertsCount);

        return new DashboardSummaryResult(
            financialCycle,
            safeDailyLimit,
            request.GoalContributedTotal,
            request.OpenAlertsCount,
            alerts,
            DateTimeOffset.UtcNow);
    }

    private static IReadOnlyCollection<DashboardAlertItem> BuildAlerts(
        SafeDailyLimitResult safeDailyLimit,
        FinancialCycleResult financialCycle,
        int openAlertsCount)
    {
        var alerts = new List<DashboardAlertItem>();

        if (safeDailyLimit.SafeDailyLimit <= 0m)
        {
            alerts.Add(new DashboardAlertItem(
                AlertType.LowBalance,
                "Margem zerada",
                "O saldo disponível do ciclo está zerado ou negativo."));
        }

        if (financialCycle.DaysRemaining <= 3)
        {
            alerts.Add(new DashboardAlertItem(
                AlertType.General,
                "Poucos dias no ciclo",
                "Faltam poucos dias para fechar o ciclo financeiro atual."));
        }

        if (openAlertsCount > 0)
        {
            alerts.Add(new DashboardAlertItem(
                AlertType.General,
                "Alertas pendentes",
                $"Você tem {openAlertsCount} alerta(s) sem leitura."));
        }

        return alerts;
    }
}
