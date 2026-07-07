namespace AssistenteGastoDiario.Application.DTOs.Incomes;

public sealed record IncomeResponse(
    Guid Id,
    Guid UserId,
    Guid? CategoryId,
    string Description,
    decimal Amount,
    DateOnly ReceivedOn,
    bool IsRecurring,
    string? Notes,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
