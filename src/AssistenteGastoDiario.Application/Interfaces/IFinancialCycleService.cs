using AssistenteGastoDiario.Application.DTOs.FinancialCycles;

namespace AssistenteGastoDiario.Application.Interfaces;

public interface IFinancialCycleService
{
    FinancialCycleResult Calculate(FinancialCycleCalculationRequest request);
}
