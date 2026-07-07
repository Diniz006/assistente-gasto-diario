using AssistenteGastoDiario.Application.DTOs.FinancialGoals;

namespace AssistenteGastoDiario.Application.Interfaces;

public interface IFinancialGoalService
{
    Task<IReadOnlyCollection<FinancialGoalResponse>> ListAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<FinancialGoalResponse?> GetByIdAsync(Guid userId, Guid financialGoalId, CancellationToken cancellationToken = default);
    Task<FinancialGoalResponse> CreateAsync(Guid userId, CreateFinancialGoalRequest request, CancellationToken cancellationToken = default);
    Task<FinancialGoalResponse?> UpdateAsync(Guid userId, Guid financialGoalId, UpdateFinancialGoalRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid userId, Guid financialGoalId, CancellationToken cancellationToken = default);
}
