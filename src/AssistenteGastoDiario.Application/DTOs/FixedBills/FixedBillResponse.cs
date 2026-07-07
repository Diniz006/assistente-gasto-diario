using AssistenteGastoDiario.Domain.Enums;

namespace AssistenteGastoDiario.Application.DTOs.FixedBills;

public sealed record FixedBillResponse(
    Guid Id,
    Guid UserId,
    Guid? CategoryId,
    string Name,
    decimal Amount,
    int DueDay,
    FixedBillStatus Status,
    DateOnly? PaymentDate,
    bool IsRecurringMonthly,
    bool AutoIncludeInCycle,
    string? Notes,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
