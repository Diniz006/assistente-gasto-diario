using System.Globalization;
using AssistenteGastoDiario.Application.DTOs.SafeDailyLimits;
using AssistenteGastoDiario.Application.Interfaces;

namespace AssistenteGastoDiario.Application.Services;

public sealed class SafeDailyLimitService : ISafeDailyLimitService
{
    private static readonly CultureInfo BrazilianCulture = CultureInfo.GetCultureInfo("pt-BR");

    public SafeDailyLimitResult Calculate(SafeDailyLimitCalculationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.DaysRemaining < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(request.DaysRemaining), "DaysRemaining cannot be negative.");
        }

        var availableBalance = RoundMoney(
            request.IncomeTotal - request.FixedBillsTotal - request.GoalPlannedTotal - request.ExpensesTotal);

        var safeDailyLimit = availableBalance <= 0m || request.DaysRemaining == 0
            ? 0m
            : RoundMoney(availableBalance / request.DaysRemaining);

        var message = safeDailyLimit <= 0m
            ? "Hoje você não tem margem disponível para novos gastos sem comprometer contas e metas."
            : string.Format(BrazilianCulture, "Hoje você pode gastar até {0:C2} sem comprometer suas contas e metas.", safeDailyLimit);

        return new SafeDailyLimitResult(
            RoundMoney(request.IncomeTotal),
            RoundMoney(request.FixedBillsTotal),
            RoundMoney(request.GoalPlannedTotal),
            RoundMoney(request.ExpensesTotal),
            availableBalance,
            request.DaysRemaining,
            safeDailyLimit,
            message);
    }

    private static decimal RoundMoney(decimal value) =>
        decimal.Round(value, 2, MidpointRounding.AwayFromZero);
}
