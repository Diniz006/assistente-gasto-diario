using AssistenteGastoDiario.Application.DTOs.FinancialSettings;

namespace AssistenteGastoDiario.Application.Interfaces;

public interface IFinancialSettingService
{
    Task<FinancialSettingResponse?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<FinancialSettingResponse> UpsertAsync(Guid userId, UpsertFinancialSettingRequest request, CancellationToken cancellationToken = default);
}
