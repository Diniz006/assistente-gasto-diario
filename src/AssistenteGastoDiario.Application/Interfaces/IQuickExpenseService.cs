using AssistenteGastoDiario.Application.DTOs.QuickExpenses;

namespace AssistenteGastoDiario.Application.Interfaces;

public interface IQuickExpenseService
{
    Task<QuickExpenseResponse> CreateAsync(
        Guid userId,
        CreateQuickExpenseRequest request,
        CancellationToken cancellationToken = default);
}
