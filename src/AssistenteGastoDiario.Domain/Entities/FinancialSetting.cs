using AssistenteGastoDiario.Domain.Common;

namespace AssistenteGastoDiario.Domain.Entities;

public sealed class FinancialSetting : UserScopedEntity
{
    public decimal MonthlyIncomeDefault { get; set; }
    public int CycleStartDay { get; set; }
    public string CurrencyCode { get; set; } = "BRL";
    public int? MonthClosureDay { get; set; }

    public User User { get; set; } = null!;
}
