using AssistenteGastoDiario.Domain.Enums;

namespace AssistenteGastoDiario.Application.DTOs.Expenses;

public sealed record ExpenseResponse(
    Guid Id,
    Guid UserId,
    Guid CategoryId,
    string Description,
    decimal Amount,
    DateOnly SpentOn,
    PaymentMethod PaymentMethod,
    string? Notes,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
