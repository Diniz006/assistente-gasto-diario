using AssistenteGastoDiario.Domain.Common;

namespace AssistenteGastoDiario.Domain.Entities;

public sealed class CycleSummary : UserScopedEntity
{
    public DateOnly CycleStartDate { get; set; }
    public DateOnly CycleEndDate { get; set; }
    public decimal IncomeTotal { get; set; }
    public decimal FixedBillsTotal { get; set; }
    public decimal ExpensesTotal { get; set; }
    public decimal GoalPlannedTotal { get; set; }
    public decimal GoalContributedTotal { get; set; }
    public decimal AvailableBalance { get; set; }
    public decimal SafeDailyLimit { get; set; }
    public int DaysInCycle { get; set; }
    public int? DaysRemainingAtClose { get; set; }
    public DateTimeOffset? ClosedAt { get; set; }

    public User User { get; set; } = null!;
}
