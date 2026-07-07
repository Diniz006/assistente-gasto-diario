namespace AssistenteGastoDiario.Application.DTOs.FinancialSettings;

public sealed record FinancialSettingResponse(
    Guid Id,
    Guid UserId,
    decimal MonthlyIncomeDefault,
    int CycleStartDay,
    string CurrencyCode,
    int? MonthClosureDay,
    DateTimeOffset CreatedAt,
    DateTimeOffset UpdatedAt);
