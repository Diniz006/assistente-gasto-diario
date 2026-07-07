using System.ComponentModel.DataAnnotations;
using AssistenteGastoDiario.Domain.Enums;

namespace AssistenteGastoDiario.Application.DTOs.QuickExpenses;

public sealed record CreateQuickExpenseRequest(
    [Required, MaxLength(160)] string Description,
    [Range(0.01, 9999999999.99)] decimal Amount,
    Guid? CategoryId = null,
    DateOnly? SpentOn = null,
    PaymentMethod PaymentMethod = PaymentMethod.Other,
    string? Notes = null);
