using AssistenteGastoDiario.Domain.Common;
using AssistenteGastoDiario.Domain.Enums;

namespace AssistenteGastoDiario.Domain.Entities;

public sealed class FinancialGoal : UserScopedEntity
{
    public Guid? CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal TargetAmount { get; set; }
    public decimal CurrentAmount { get; set; }
    public decimal MonthlyPlannedAmount { get; set; }
    public DateOnly? TargetDate { get; set; }
    public GoalPriority Priority { get; set; } = GoalPriority.Medium;
    public GoalStatus Status { get; set; } = GoalStatus.Active;
    public string? Notes { get; set; }

    public User User { get; set; } = null!;
    public Category? Category { get; set; }
    public ICollection<GoalContribution> GoalContributions { get; set; } = new List<GoalContribution>();
}
