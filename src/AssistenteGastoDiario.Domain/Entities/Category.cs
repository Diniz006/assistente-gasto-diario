using AssistenteGastoDiario.Domain.Common;
using AssistenteGastoDiario.Domain.Enums;

namespace AssistenteGastoDiario.Domain.Entities;

public sealed class Category : UserScopedEntity
{
    public string Name { get; set; } = string.Empty;
    public CategoryType Type { get; set; }
    public string? Color { get; set; }
    public string? Icon { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; } = true;

    public User User { get; set; } = null!;
    public ICollection<Income> Incomes { get; set; } = new List<Income>();
    public ICollection<FixedBill> FixedBills { get; set; } = new List<FixedBill>();
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    public ICollection<FinancialGoal> FinancialGoals { get; set; } = new List<FinancialGoal>();
    public ICollection<MonthlyBudget> MonthlyBudgets { get; set; } = new List<MonthlyBudget>();
}
