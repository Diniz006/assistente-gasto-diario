using AssistenteGastoDiario.Domain.Common;
using AssistenteGastoDiario.Domain.Enums;

namespace AssistenteGastoDiario.Domain.Entities;

public sealed class Alert : UserScopedEntity
{
    public AlertType Type { get; set; } = AlertType.General;
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public string? RelatedEntityType { get; set; }
    public Guid? RelatedEntityId { get; set; }
    public DateTimeOffset? ReadAt { get; set; }

    public User User { get; set; } = null!;
}
