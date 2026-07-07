using AssistenteGastoDiario.Application.DTOs.FinancialCycles;
using AssistenteGastoDiario.Application.Interfaces;

namespace AssistenteGastoDiario.Application.Services;

public sealed class FinancialCycleService : IFinancialCycleService
{
    public FinancialCycleResult Calculate(FinancialCycleCalculationRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.CycleStartDay < 1 || request.CycleStartDay > 31)
        {
            throw new ArgumentOutOfRangeException(nameof(request.CycleStartDay), "CycleStartDay must be between 1 and 31.");
        }

        var cycleStartDate = GetCycleStartDate(request.ReferenceDate, request.CycleStartDay);
        var nextCycleStartDate = GetCycleStartDate(request.ReferenceDate >= cycleStartDate
            ? request.ReferenceDate.AddMonths(1)
            : request.ReferenceDate, request.CycleStartDay);

        if (request.ReferenceDate < cycleStartDate)
        {
            cycleStartDate = GetCycleStartDate(request.ReferenceDate.AddMonths(-1), request.CycleStartDay);
            nextCycleStartDate = GetCycleStartDate(request.ReferenceDate, request.CycleStartDay);
        }

        var cycleEndDate = nextCycleStartDate.AddDays(-1);
        var daysInCycle = cycleEndDate.DayNumber - cycleStartDate.DayNumber + 1;
        var daysRemaining = Math.Max(cycleEndDate.DayNumber - request.ReferenceDate.DayNumber + 1, 0);

        return new FinancialCycleResult(
            cycleStartDate,
            cycleEndDate,
            daysInCycle,
            daysRemaining,
            nextCycleStartDate);
    }

    private static DateOnly GetCycleStartDate(DateOnly referenceDate, int cycleStartDay)
    {
        var day = Math.Min(cycleStartDay, DateTime.DaysInMonth(referenceDate.Year, referenceDate.Month));
        return new DateOnly(referenceDate.Year, referenceDate.Month, day);
    }
}
