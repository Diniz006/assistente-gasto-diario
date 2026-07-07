using AssistenteGastoDiario.Application.DTOs.Incomes;

namespace AssistenteGastoDiario.Application.Interfaces;

public interface IIncomeService
{
    Task<IReadOnlyCollection<IncomeResponse>> ListByPeriodAsync(Guid userId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default);
    Task<IncomeResponse?> GetByIdAsync(Guid userId, Guid incomeId, CancellationToken cancellationToken = default);
    Task<IncomeResponse> CreateAsync(Guid userId, CreateIncomeRequest request, CancellationToken cancellationToken = default);
    Task<IncomeResponse?> UpdateAsync(Guid userId, Guid incomeId, UpdateIncomeRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid userId, Guid incomeId, CancellationToken cancellationToken = default);
}
