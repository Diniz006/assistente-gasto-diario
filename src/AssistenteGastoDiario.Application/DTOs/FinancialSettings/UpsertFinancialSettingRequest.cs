using System.ComponentModel.DataAnnotations;

namespace AssistenteGastoDiario.Application.DTOs.FinancialSettings;

public sealed record UpsertFinancialSettingRequest(
    [Range(0, 9999999999.99)] decimal MonthlyIncomeDefault,
    [Range(1, 31)] int CycleStartDay,
    [Required, StringLength(3, MinimumLength = 3)] string CurrencyCode = "BRL",
    [Range(1, 31)] int? MonthClosureDay = null);
