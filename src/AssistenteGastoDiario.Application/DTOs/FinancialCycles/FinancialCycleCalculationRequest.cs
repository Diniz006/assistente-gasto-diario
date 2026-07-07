namespace AssistenteGastoDiario.Application.DTOs.FinancialCycles;

public sealed record FinancialCycleCalculationRequest(
    DateOnly ReferenceDate,
    int CycleStartDay,
    int? MonthClosureDay = null);
