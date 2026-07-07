using AssistenteGastoDiario.Domain.Common;

namespace AssistenteGastoDiario.Domain.Entities;

public sealed class GoalContribution : UserScopedEntity
{
    public Guid FinancialGoalId { get; set; }
    public decimal Amount { get; set; }
    public DateOnly ContributedOn { get; set; }
    public string? Notes { get; set; }

    public User User { get; set; } = null!;
    public FinancialGoal FinancialGoal { get; set; } = null!;
}
