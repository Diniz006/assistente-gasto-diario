using AssistenteGastoDiario.Domain.Common;

namespace AssistenteGastoDiario.Domain.Entities;

public sealed class Income : UserScopedEntity
{
    public Guid? CategoryId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateOnly ReceivedOn { get; set; }
    public bool IsRecurring { get; set; }
    public string? Notes { get; set; }

    public User User { get; set; } = null!;
    public Category? Category { get; set; }
}
