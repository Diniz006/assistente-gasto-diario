using System.ComponentModel.DataAnnotations;

namespace AssistenteGastoDiario.Application.DTOs.Incomes;

public sealed record CreateIncomeRequest(
    Guid? CategoryId,
    [Required, MaxLength(160)] string Description,
    [Range(0.01, 9999999999.99)] decimal Amount,
    DateOnly ReceivedOn,
    bool IsRecurring = false,
    string? Notes = null);
