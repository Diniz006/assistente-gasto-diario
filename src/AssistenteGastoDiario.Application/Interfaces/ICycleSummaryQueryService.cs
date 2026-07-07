using AssistenteGastoDiario.Application.DTOs.CycleSummaries;

namespace AssistenteGastoDiario.Application.Interfaces;

public interface ICycleSummaryQueryService
{
    Task<CycleSummaryResponse?> GetCurrentAsync(
        Guid userId,
        DateOnly referenceDate,
        CancellationToken cancellationToken = default);
}
