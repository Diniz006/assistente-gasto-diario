using AssistenteGastoDiario.Domain.Common;
using AssistenteGastoDiario.Domain.Enums;

namespace AssistenteGastoDiario.Domain.Entities;

public sealed class Expense : UserScopedEntity
{
    public Guid CategoryId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateOnly SpentOn { get; set; }
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Other;
    public string? Notes { get; set; }

    public User User { get; set; } = null!;
    public Category Category { get; set; } = null!;
}
