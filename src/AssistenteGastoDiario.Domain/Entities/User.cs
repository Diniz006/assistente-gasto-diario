using AssistenteGastoDiario.Domain.Common;

namespace AssistenteGastoDiario.Domain.Entities;

public sealed class User : EntityBase
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTimeOffset? DeletedAt { get; set; }

    public FinancialSetting? FinancialSetting { get; set; }
    public ICollection<Category> Categories { get; set; } = new List<Category>();
    public ICollection<Income> Incomes { get; set; } = new List<Income>();
    public ICollection<FixedBill> FixedBills { get; set; } = new List<FixedBill>();
    public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    public ICollection<FinancialGoal> FinancialGoals { get; set; } = new List<FinancialGoal>();
    public ICollection<GoalContribution> GoalContributions { get; set; } = new List<GoalContribution>();
    public ICollection<MonthlyBudget> MonthlyBudgets { get; set; } = new List<MonthlyBudget>();
    public ICollection<Alert> Alerts { get; set; } = new List<Alert>();
    public ICollection<CycleSummary> CycleSummaries { get; set; } = new List<CycleSummary>();
}
