using AssistenteGastoDiario.Application.DTOs.SafeDailyLimits;

namespace AssistenteGastoDiario.Application.Interfaces;

public interface ISafeDailyLimitService
{
    SafeDailyLimitResult Calculate(SafeDailyLimitCalculationRequest request);
}
