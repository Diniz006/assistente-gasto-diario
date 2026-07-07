using System.ComponentModel.DataAnnotations;
using AssistenteGastoDiario.Domain.Enums;

namespace AssistenteGastoDiario.Application.DTOs.Expenses;

public sealed record UpdateExpenseRequest(
    Guid CategoryId,
    [Required, MaxLength(160)] string Description,
    [Range(0.01, 9999999999.99)] decimal Amount,
    DateOnly SpentOn,
    PaymentMethod PaymentMethod,
    string? Notes);
