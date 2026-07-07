using AssistenteGastoDiario.Domain.Common;
using AssistenteGastoDiario.Domain.Enums;

namespace AssistenteGastoDiario.Domain.Entities;

public sealed class FixedBill : UserScopedEntity
{
    public Guid? CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int DueDay { get; set; }
    public FixedBillStatus Status { get; set; } = FixedBillStatus.Pending;
    public DateOnly? PaymentDate { get; set; }
    public bool IsRecurringMonthly { get; set; } = true;
    public bool AutoIncludeInCycle { get; set; } = true;
    public string? Notes { get; set; }

    public User User { get; set; } = null!;
    public Category? Category { get; set; }
}
