using AssistenteGastoDiario.Domain.Common;

namespace AssistenteGastoDiario.Domain.Entities;

public sealed class MonthlyBudget : UserScopedEntity
{
    public Guid CategoryId { get; set; }
    public DateOnly CycleStartDate { get; set; }
    public DateOnly CycleEndDate { get; set; }
    public decimal BudgetAmount { get; set; }
    public decimal SpentAmount { get; set; }
    public string? Notes { get; set; }

    public User User { get; set; } = null!;
    public Category Category { get; set; } = null!;
}
