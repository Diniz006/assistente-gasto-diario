namespace AssistenteGastoDiario.Application.DTOs.FinancialCycles;

public sealed record FinancialCycleResult(
    DateOnly CycleStartDate,
    DateOnly CycleEndDate,
    int DaysInCycle,
    int DaysRemaining,
    DateOnly NextCycleStartDate);
