using AssistenteGastoDiario.Application.DTOs.Expenses;

namespace AssistenteGastoDiario.Application.Interfaces;

public interface IExpenseService
{
    Task<IReadOnlyCollection<ExpenseResponse>> ListByPeriodAsync(Guid userId, DateOnly startDate, DateOnly endDate, CancellationToken cancellationToken = default);
    Task<ExpenseResponse?> GetByIdAsync(Guid userId, Guid expenseId, CancellationToken cancellationToken = default);
    Task<ExpenseResponse> CreateAsync(Guid userId, CreateExpenseRequest request, CancellationToken cancellationToken = default);
    Task<ExpenseResponse?> UpdateAsync(Guid userId, Guid expenseId, UpdateExpenseRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid userId, Guid expenseId, CancellationToken cancellationToken = default);
}
