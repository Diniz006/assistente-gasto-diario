using AssistenteGastoDiario.Application.DTOs.Dashboard;

namespace AssistenteGastoDiario.Application.Interfaces;

public interface IDashboardQueryService
{
    Task<DashboardSummaryResult?> GetCurrentAsync(
        Guid userId,
        DateOnly referenceDate,
        CancellationToken cancellationToken = default);
}
